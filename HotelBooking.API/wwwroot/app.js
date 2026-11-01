(() => {
  const $ = (selector, root = document) => root.querySelector(selector);
  const $$ = (selector, root = document) => [...root.querySelectorAll(selector)];
  const storeKey = 'hotelyar.session';
  const state = { hotels: [], rooms: {}, authMode: 'login', dates: { checkIn: '', checkOut: '' }, toastTimer: null };
  const readSession = () => { try { return JSON.parse(localStorage.getItem(storeKey) || '{}'); } catch { return {}; } };
  let session = readSession();
  const saveSession = value => { session = value; localStorage.setItem(storeKey, JSON.stringify(value)); updateAuthUI(); };
  const clearSession = () => { session = {}; localStorage.removeItem(storeKey); updateAuthUI(); };
  const decodeToken = token => { try { return JSON.parse(decodeURIComponent(escape(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/'))))); } catch { return {}; } };
  const claims = () => decodeToken(session.accessToken || '');
  const claim = (...names) => { const c = claims(); for (const name of names) if (c[name]) return c[name]; return ''; };
  const isAdmin = () => String(claim('role', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role')).toLowerCase() === 'admin';
  const isLoggedIn = () => !!session.accessToken;
  const showToast = (message, error = false) => { const toast = $('#toast'); toast.textContent = message; toast.classList.toggle('error', error); toast.classList.add('show'); clearTimeout(state.toastTimer); state.toastTimer = setTimeout(() => toast.classList.remove('show'), 3600); };
  const messageFrom = body => {
    if (typeof body === 'string') return body;
    if (body?.message || body?.Message) return body.message || body.Message;
    if (Array.isArray(body?.errors || body?.Errors)) return (body.errors || body.Errors).map(x => x.message || x.Message || '').filter(Boolean).join('، ');
    return body?.title || body?.Title || body?.detail || body?.Detail || 'در ارتباط با سرور مشکلی پیش آمد.';
  };
  async function request(path, options = {}, retried = false) {
    const headers = new Headers(options.headers || {});
    if (session.accessToken) headers.set('Authorization', `Bearer ${session.accessToken}`);
    if (options.body && !(options.body instanceof FormData) && typeof options.body !== 'string') {
      headers.set('Content-Type', 'application/json'); options.body = JSON.stringify(options.body);
    }
    const response = await fetch(path, { ...options, headers });
    if (response.status === 401 && session.refreshToken && !retried && !path.includes('refresh-token')) {
      try {
        const refreshResponse = await fetch('/api/Auth/refresh-token', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ refreshToken: session.refreshToken }) });
        if (refreshResponse.ok) { const fresh = await refreshResponse.json(); saveSession({ ...session, accessToken: fresh.accessToken, refreshToken: fresh.refreshToken, expiresAt: fresh.expiresAt }); return request(path, options, true); }
      } catch { /* The original response gives the user the useful error. */ }
      clearSession();
    }
    if (!response.ok) {
      const text = await response.text(); let body = text;
      try { body = text ? JSON.parse(text) : ''; } catch { /* Keep the plain response. */ }
      throw new Error(messageFrom(body) || `خطای سرور (${response.status})`);
    }
    if (response.status === 204) return null;
    const text = await response.text();
    if (!text) return null;
    try { return JSON.parse(text); } catch { return text; }
  }
  const formObject = form => Object.fromEntries(new FormData(form).entries());
  const esc = value => String(value ?? '').replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[c]);
  const imageUrl = url => { if (!url) return ''; try { return new URL(url, location.origin).href; } catch { return ''; } };
  const hotelImage = hotel => (hotel.images || []).find(i => i.isMain)?.imageUrl || hotel.images?.[0]?.imageUrl || '';
  const galleryMarkup = (images, group, alt) => {
    const photos = images || [];
    const main = photos.find(image => image.isMain) || photos[0];
    return `<div class="detail-gallery"><div class="gallery-main">${main ? `<img data-gallery-target="${group}" src="${esc(imageUrl(main.imageUrl))}" alt="${esc(alt)}" loading="lazy">` : '<div class="gallery-placeholder">تصویری برای نمایش ثبت نشده است</div>'}</div>${photos.length > 1 ? `<div class="gallery-thumbs">${photos.map((photo, index) => `<button type="button" class="gallery-thumb${photo === main ? ' active' : ''}" data-gallery-group="${group}" data-gallery-image="${esc(imageUrl(photo.imageUrl))}" aria-label="نمایش تصویر ${index + 1}"><img src="${esc(imageUrl(photo.imageUrl))}" alt="" loading="lazy"></button>`).join('')}</div>` : ''}</div>`;
  };
  const price = value => `${Number(value || 0).toLocaleString('fa-IR')} تومان`;
  const dateLabel = value => { if (!value) return '—'; const d = new Date(value); return Number.isNaN(+d) ? esc(value) : new Intl.DateTimeFormat('fa-IR', { year: 'numeric', month: 'short', day: 'numeric' }).format(d); };
  const hotelById = id => state.hotels.find(h => Number(h.id) === Number(id));
  const roomDetailHref = (hotelId, roomId, checkIn, checkOut, guests) => {
    const params = new URLSearchParams({ hotelId, roomId });
    if (checkIn) params.set('checkIn', checkIn);
    if (checkOut) params.set('checkOut', checkOut);
    if (guests) params.set('guests', guests);
    return `/room.html?${params}`;
  };

  function updateAuthUI() {
    $('#auth-button').classList.toggle('hidden', isLoggedIn()); $('#logout-button').classList.toggle('hidden', !isLoggedIn());
    $('#user-label').textContent = isLoggedIn() ? `سلام، ${claim('unique_name', 'name', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name') || 'همراه عزیز'}` : '';
    $$('.auth-link').forEach(el => el.classList.toggle('hidden', !isLoggedIn()));
    $('.admin-link')?.classList.toggle('hidden', !isAdmin());
    $('#admin')?.classList.toggle('hidden', !isAdmin());
    $('#admin-guest')?.classList.toggle('hidden', isAdmin());
    $('#bookings')?.classList.toggle('hidden', !isLoggedIn()); $('#profile')?.classList.toggle('hidden', !isLoggedIn());
    $('#account-guest')?.classList.toggle('hidden', isLoggedIn());
  }
  function renderHotels() {
    const text = $('#hotel-filter').value.trim().toLocaleLowerCase('fa'); const stars = Number($('#star-filter').value || 0);
    const matches = state.hotels.filter(h => (!text || `${h.name} ${h.city} ${h.address}`.toLocaleLowerCase('fa').includes(text)) && Number(h.starRating) >= stars);
    $('#hotel-count').textContent = state.hotels.length.toLocaleString('fa-IR');
    $('#hotel-empty').classList.toggle('hidden', matches.length > 0);
    $('#hotel-grid').innerHTML = matches.map(h => {
      const rooms = state.rooms[h.id] || []; const img = hotelImage(h); const photo = img ? `<img src="${esc(imageUrl(img))}" alt="نمای هتل ${esc(h.name)}" loading="lazy" onerror="this.remove()">` : '';
      return `<article class="hotel-card"><div class="hotel-photo">${photo || '<div class="photo-placeholder">⌂</div>'}<span class="photo-city">${esc(h.city)}، ${esc(h.country)}</span></div><div class="hotel-info"><div class="hotel-title-line"><h3>${esc(h.name)}</h3><span class="rating" aria-label="${esc(h.starRating)} ستاره">${'★'.repeat(Math.max(0, Math.min(5, Number(h.starRating) || 0)))}</span></div><p class="hotel-location">${esc(h.address || h.city)}</p><p class="hotel-description">${esc(h.description || 'برای آشنایی بیشتر با اتاق‌ها و امکانات این اقامتگاه، جزئیات را ببینید.')}</p><div class="hotel-info-bottom"><span class="rooms-count"><strong>${rooms.length.toLocaleString('fa-IR')}</strong> اتاق</span><button class="small-button" data-open-rooms="${h.id}">دیدن اتاق‌ها ←</button></div></div></article>`;
    }).join('');
    $$('[data-open-rooms]').forEach(button => button.addEventListener('click', () => {
      const params = new URLSearchParams({ id: button.dataset.openRooms });
      if ($('#search-checkin')?.value) params.set('checkIn', $('#search-checkin').value);
      if ($('#search-checkout')?.value) params.set('checkOut', $('#search-checkout').value);
      if ($('#search-guests')?.value) params.set('guests', $('#search-guests').value);
      location.href = `/hotel.html?${params}`;
    }));
  }
  async function loadHotels() {
    if ($('#hotel-grid')) $('#hotel-grid').innerHTML = '<div class="loading-card">در حال دریافت هتل‌ها…</div>';
    try {
      state.hotels = await request('/api/Hotel') || [];
      if ($('#hotel-grid') || $('#admin-hotel-list')) await Promise.all(state.hotels.map(async hotel => { try { state.rooms[hotel.id] = await request(`/api/Hotel/${hotel.id}/rooms`) || []; } catch { state.rooms[hotel.id] = []; } }));
      if ($('#hotel-grid')) renderHotels();
      fillHotelSelect(); if (isAdmin() && $('#admin-hotel-list')) renderAdminHotels();
    } catch (error) { if ($('#hotel-grid')) { $('#hotel-grid').innerHTML = `<div class="empty-state">${esc(error.message)} <button class="small-button" id="retry-hotels">تلاش دوباره</button></div>`; $('#retry-hotels')?.addEventListener('click', loadHotels); } else showToast(error.message, true); }
  }
  function fillHotelSelect() {
    const select = $('#room-hotel-select'); if (!select) return; const previous = select.value;
    select.innerHTML = state.hotels.length ? state.hotels.map(h => `<option value="${h.id}">${esc(h.name)} — ${esc(h.city)}</option>`).join('') : '<option value="">ابتدا یک هتل ثبت کنید</option>';
    if (state.hotels.some(h => String(h.id) === previous)) select.value = previous;
  }
  function setDateDefaults() {
    const today = new Date(); const tomorrow = new Date(today); tomorrow.setDate(today.getDate() + 1);
    const asInput = d => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
    $('#search-checkin').min = asInput(tomorrow); $('#search-checkout').min = asInput(tomorrow);
    $('#search-checkin').value = ''; $('#search-checkout').value = '';
    $('#search-checkin').addEventListener('change', () => { const date = new Date(`${$('#search-checkin').value}T00:00:00`); date.setDate(date.getDate() + 1); $('#search-checkout').min = asInput(date); if ($('#search-checkout').value <= $('#search-checkin').value) $('#search-checkout').value = ''; });
  }
  async function openRooms(hotelId) {
    const hotel = hotelById(hotelId); if (!hotel) return;
    const params = new URLSearchParams(location.search);
    const selectedCheckIn = $('#search-checkin')?.value || params.get('checkIn') || ''; const selectedCheckOut = $('#search-checkout')?.value || params.get('checkOut') || '';
    const dialog = $('#room-dialog'); const content = $('#room-page-content') || $('#room-dialog-content');
    content.innerHTML = `<span class="eyebrow">${esc(hotel.city)} · ${'★'.repeat(Math.min(5, Number(hotel.starRating) || 0))}</span><h2>${esc(hotel.name)}</h2>${galleryMarkup(hotel.images, `hotel-${hotel.id}`, `تصاویر هتل ${hotel.name}`)}<p class="hotel-location">${esc(hotel.address)}${hotel.phoneNumber ? ` · ${esc(hotel.phoneNumber)}` : ''}</p><p>${esc(hotel.description || '')}</p><div class="stay-picker"><label>تاریخ ورود<input id="room-checkin" type="date" value="${esc(selectedCheckIn)}" required></label><label>تاریخ خروج<input id="room-checkout" type="date" value="${esc(selectedCheckOut)}" required></label><label>تعداد نفرات<select id="room-guests"><option value="1">۱ نفر</option><option value="2">۲ نفر</option><option value="3">۳ نفر</option><option value="4">۴ نفر</option><option value="5">۵ نفر یا بیشتر</option></select></label><button id="check-room-dates" class="button button-accent">بررسی اتاق‌های آزاد</button></div><h3 class="room-list-title">اتاق‌های این هتل</h3><div class="room-list"><div class="empty-state">در حال دریافت اتاق‌های هتل…</div></div>`;
    if (dialog && !dialog.open) dialog.showModal();
    try {
      state.rooms[hotelId] = await request(`/api/Hotel/${hotelId}/rooms`) || [];
      if ($('#hotel-grid')) renderHotels();
      renderHotelRoomsBeforeAvailability(hotelId);
      const today = new Date(); const tomorrow = new Date(today); tomorrow.setDate(today.getDate() + 1);
      const asInput = date => `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
      $('#room-checkin').min = asInput(tomorrow); $('#room-checkout').min = asInput(tomorrow);
      $('#room-guests').value = $('#search-guests')?.value || params.get('guests') || '2';
      $('#room-checkin').addEventListener('change', () => {
        const date = new Date(`${$('#room-checkin').value}T00:00:00`); date.setDate(date.getDate() + 1);
        $('#room-checkout').min = asInput(date);
        if ($('#room-checkout').value <= $('#room-checkin').value) $('#room-checkout').value = '';
        invalidateRoomAvailability();
      });
      $('#room-checkout').addEventListener('change', invalidateRoomAvailability);
      $('#room-guests').addEventListener('change', invalidateRoomAvailability);
      $('#check-room-dates').addEventListener('click', () => searchAvailableRooms(hotelId));
    } catch (error) { content.querySelector('.room-list').innerHTML = `<div class="empty-state">${esc(error.message)}</div>`; }
  }
  function renderHotelRoomsBeforeAvailability(hotelId) {
    const rooms = state.rooms[hotelId] || [];
    const list = $('.room-list');
    list.innerHTML = rooms.length ? rooms.map(room => {
      const isMaintenance = String(room.status).toLowerCase() === 'maintenance';
      return `<article class="room-item room-item-with-gallery">${galleryMarkup(room.images, `room-${room.id}`, `تصاویر اتاق ${room.roomNumber}`)}<div class="room-summary"><strong>اتاق ${Number(room.roomNumber).toLocaleString('fa-IR')} · ${esc(room.roomType || 'اتاق')}</strong><small>ظرفیت ${Number(room.capacity).toLocaleString('fa-IR')} نفر · ${isMaintenance ? 'در حال تعمیر' : 'برای دیدن جزئیات و رزرو، اتاق را انتخاب کنید'}</small><span class="room-price">${price(room.pricePerNight)}<small>هر شب</small></span>${isMaintenance ? '<span class="status-pill">در حال تعمیر</span>' : `<a class="small-button" href="${roomDetailHref(hotelId, room.id, $('#room-checkin').value, $('#room-checkout').value, $('#room-guests').value)}">مشاهدهٔ جزئیات اتاق</a>`}</div></article>`;
    }).join('') : '<div class="empty-state">برای این هتل هنوز اتاقی ثبت نشده است.</div>';
  }
  async function searchAvailableRooms(hotelId) {
    const checkIn = $('#room-checkin').value; const checkOut = $('#room-checkout').value;
    const list = $('.room-list');
    if (!checkIn || !checkOut || checkOut <= checkIn) {
      list.innerHTML = '<div class="empty-state">لطفاً تاریخ ورود و خروج را انتخاب کنید؛ تاریخ خروج باید بعد از ورود باشد.</div>';
      return;
    }
    list.innerHTML = '<div class="loading-card">در حال بررسی ظرفیت اتاق‌ها برای تاریخ انتخابی…</div>';
    const rooms = state.rooms[hotelId] || []; const hotel = hotelById(hotelId);
    const minCapacity = Number($('#room-guests').value || 1);
    const checkInIso = new Date(`${checkIn}T12:00:00`).toISOString();
    const checkOutIso = new Date(`${checkOut}T12:00:00`).toISOString();
    let availabilityError = null;
    const available = (await Promise.all(rooms.filter(room => String(room.status).toLowerCase() !== 'maintenance' && Number(room.capacity) >= minCapacity).map(async room => {
      try {
        const query = `checkInDate=${encodeURIComponent(checkInIso)}&checkOutDate=${encodeURIComponent(checkOutIso)}`;
        const result = await request(`/api/Room/${room.id}/availability?${query}`);
        return result.isAvailable ? room : null;
      } catch (error) { availabilityError = error; return null; }
    }))).filter(Boolean);
    const nights = Math.round((new Date(`${checkOut}T00:00:00`) - new Date(`${checkIn}T00:00:00`)) / 86400000);
    list.innerHTML = available.length ? available.map(room => `<article class="room-item room-item-with-gallery">${galleryMarkup(room.images, `room-${room.id}`, `تصاویر اتاق ${room.roomNumber}`)}<div class="room-summary"><strong>اتاق ${Number(room.roomNumber).toLocaleString('fa-IR')} · ${esc(room.roomType || 'اتاق')}</strong><small>ظرفیت ${Number(room.capacity).toLocaleString('fa-IR')} نفر · ${esc(hotel?.name || '')}</small><span class="room-price">${price(room.pricePerNight)}<small>هر شب · جمع ${price(room.pricePerNight * nights)}</small></span><a class="small-button" href="${roomDetailHref(hotelId, room.id, checkIn, checkOut, $('#room-guests').value)}">انتخاب اتاق و دیدن جزئیات</a></div></article>`).join('') : `<div class="empty-state">${availabilityError ? esc(availabilityError.message) : 'برای این تاریخ و تعداد نفرات، اتاق آزادی پیدا نشد.'}</div>`;
  }
  async function loadRoomDetails(hotelId, roomId) {
    const content = $('#room-detail-content');
    if (!hotelId || !roomId) { content.innerHTML = '<div class="empty-state">شناسهٔ هتل یا اتاق معتبر نیست.</div>'; return; }
    const params = new URLSearchParams(location.search);
    try {
      const [hotel, rooms] = await Promise.all([request(`/api/Hotel/${hotelId}`), request(`/api/Hotel/${hotelId}/rooms`)]);
      const room = (rooms || []).find(item => Number(item.id) === roomId);
      if (!room || !hotel) { content.innerHTML = '<div class="empty-state">اطلاعات هتل یا اتاق پیدا نشد.</div>'; return; }
      state.rooms[hotelId] = rooms || [];
      const checkIn = params.get('checkIn') || '';
      const checkOut = params.get('checkOut') || '';
      const guests = params.get('guests') || '2';
      const hotelUrl = new URLSearchParams({ id: hotelId });
      if (checkIn) hotelUrl.set('checkIn', checkIn);
      if (checkOut) hotelUrl.set('checkOut', checkOut);
      if (guests) hotelUrl.set('guests', guests);
      $('#room-back-link').href = `/hotel.html?${hotelUrl}`;
      content.innerHTML = `<span class="eyebrow">جزئیات اتاق · ${esc(hotel.name)}</span><h1>اتاق ${Number(room.roomNumber).toLocaleString('fa-IR')} · ${esc(room.roomType || 'اتاق')}</h1>${galleryMarkup(room.images, `room-detail-${room.id}`, `تصاویر اتاق ${room.roomNumber}`)}<div class="room-detail-meta"><div><span>هتل</span><strong>${esc(hotel.name)}</strong></div><div><span>ظرفیت</span><strong>${Number(room.capacity).toLocaleString('fa-IR')} نفر</strong></div><div><span>قیمت هر شب</span><strong>${price(room.pricePerNight)}</strong></div><div><span>وضعیت</span><strong>${esc(room.status)}</strong></div></div><div class="stay-picker"><label>تاریخ ورود<input id="room-checkin" type="date" value="${esc(checkIn)}" required></label><label>تاریخ خروج<input id="room-checkout" type="date" value="${esc(checkOut)}" required></label><label>تعداد نفرات<select id="room-guests"><option value="1">۱ نفر</option><option value="2">۲ نفر</option><option value="3">۳ نفر</option><option value="4">۴ نفر</option><option value="5">۵ نفر یا بیشتر</option></select></label><button id="check-room-dates" class="button button-accent">بررسی موجودی اتاق</button></div><p id="room-availability-state" class="availability-state" aria-live="polite">تاریخ اقامت را انتخاب و موجودی این اتاق را بررسی کنید.</p><button id="room-book-button" class="button button-dark hidden">رزرو این اتاق</button>`;
      $('#room-guests').value = guests;
      const tomorrow = new Date(); tomorrow.setDate(tomorrow.getDate() + 1);
      const asInput = date => `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
      $('#room-checkin').min = asInput(tomorrow); $('#room-checkout').min = asInput(tomorrow);
      $('#room-checkin').addEventListener('change', () => {
        const date = new Date(`${$('#room-checkin').value}T00:00:00`); date.setDate(date.getDate() + 1);
        $('#room-checkout').min = asInput(date);
        if ($('#room-checkout').value <= $('#room-checkin').value) $('#room-checkout').value = '';
        invalidateRoomAvailability();
      });
      $('#room-checkout').addEventListener('change', invalidateRoomAvailability);
      $('#room-guests').addEventListener('change', invalidateRoomAvailability);
      $('#check-room-dates').addEventListener('click', () => checkRoomDetailAvailability(hotelId, roomId));
      $('#room-book-button').addEventListener('click', () => createBooking(hotelId, roomId));
    } catch (error) { content.innerHTML = `<div class="empty-state">${esc(error.message)}</div>`; }
  }
  async function checkRoomDetailAvailability(hotelId, roomId) {
    const checkIn = $('#room-checkin').value; const checkOut = $('#room-checkout').value;
    const stateLabel = $('#room-availability-state'); const bookButton = $('#room-book-button');
    bookButton.classList.add('hidden');
    if (!checkIn || !checkOut || checkOut <= checkIn) { stateLabel.textContent = 'تاریخ خروج باید بعد از تاریخ ورود باشد.'; stateLabel.classList.add('error'); return; }
    const room = (state.rooms[hotelId] || []).find(item => Number(item.id) === roomId);
    if (Number($('#room-guests').value) > Number(room?.capacity || 0)) { stateLabel.textContent = 'ظرفیت این اتاق برای تعداد نفرات انتخاب‌شده کافی نیست.'; stateLabel.classList.add('error'); return; }
    stateLabel.textContent = 'در حال بررسی موجودی…'; stateLabel.classList.remove('error', 'success');
    try {
      const query = `checkInDate=${encodeURIComponent(new Date(`${checkIn}T12:00:00`).toISOString())}&checkOutDate=${encodeURIComponent(new Date(`${checkOut}T12:00:00`).toISOString())}`;
      const result = await request(`/api/Room/${roomId}/availability?${query}`);
      if (!result.isAvailable) { stateLabel.textContent = 'این اتاق در تاریخ انتخابی آزاد نیست. تاریخ دیگری را امتحان کنید.'; stateLabel.classList.add('error'); return; }
      const nights = Math.round((new Date(`${checkOut}T00:00:00`) - new Date(`${checkIn}T00:00:00`)) / 86400000);
      stateLabel.textContent = `اتاق برای ${nights.toLocaleString('fa-IR')} شب آزاد است. مبلغ کل: ${price(Number(room?.pricePerNight || 0) * nights)}.`;
      stateLabel.classList.add('success'); bookButton.classList.remove('hidden');
    } catch (error) { stateLabel.textContent = error.message; stateLabel.classList.add('error'); }
  }
  function invalidateRoomAvailability() {
    $('#room-book-button')?.classList.add('hidden');
    const label = $('#room-availability-state');
    if (label) { label.textContent = 'تغییرات ثبت شد؛ موجودی را دوباره بررسی کنید.'; label.classList.remove('success', 'error'); }
  }
  async function createBooking(hotelId, roomId) {
    if (!isLoggedIn()) { $('#room-dialog')?.close(); showAuth('login'); showToast('برای ثبت رزرو ابتدا وارد حساب‌تان شوید.'); return; }
    const checkIn = $('#room-checkin').value; const checkOut = $('#room-checkout').value;
    if (!checkIn || !checkOut || checkOut <= checkIn) { showToast('تاریخ ورود و خروج را درست انتخاب کنید.', true); return; }
    try {
      await request('/api/Booking', { method: 'POST', body: { hotelId, roomId, checkInDate: new Date(`${checkIn}T12:00:00`).toISOString(), checkOutDate: new Date(`${checkOut}T12:00:00`).toISOString() } });
      if ($('#room-dialog')) $('#room-dialog').close();
      showToast('درخواست رزرو ثبت شد؛ پس از تأیید مدیریت، رزرو قطعی می‌شود.');
      if (document.body.dataset.page === 'hotel') { location.href = '/account.html#bookings'; return; }
      await loadMyBookings(); document.querySelector('#bookings').scrollIntoView({ behavior: 'smooth' });
    } catch (error) { showToast(error.message, true); }
  }
  const bookingStatuses = { Pending: 'در انتظار تأیید مدیر', Confirmed: 'رزرو قطعی', Cancelled: 'لغوشده', Completed: 'تکمیل‌شده', CheckedIn: 'اقامت در حال انجام' };
  const statusClass = value => `status-${String(value || '').toLowerCase()}`;
  async function loadMyBookings() {
    if (!isLoggedIn()) return;
    const list = $('#booking-list'); list.innerHTML = '<div class="loading-card">در حال دریافت رزروها…</div>';
    try {
      const bookings = await request('/api/Booking/my-bookings') || [];
      list.innerHTML = bookings.length ? bookings.map(b => `<article class="booking-card"><div><strong>${esc(b.hotelName)} · اتاق ${Number(b.roomNumber).toLocaleString('fa-IR')}</strong><small>${Number(b.checkOutDate ? Math.max(1, Math.round((new Date(b.checkOutDate) - new Date(b.checkInDate)) / 86400000)) : 1).toLocaleString('fa-IR')} شب اقامت</small></div><div><small>ورود ${dateLabel(b.checkInDate)}</small><small>خروج ${dateLabel(b.checkOutDate)}</small></div><div><strong>${price(b.totalPrice)}</strong><small class="status-pill ${statusClass(b.status)}">${esc(bookingStatuses[b.status] || b.status)}</small></div><span></span></article>`).join('') : '<div class="empty-state">هنوز رزروی ثبت نکرده‌اید. از بین هتل‌ها اقامتگاه‌تان را انتخاب کنید.</div>';
    } catch (error) { list.innerHTML = `<div class="empty-state">${esc(error.message)} <button class="small-button" id="retry-bookings">تلاش دوباره</button></div>`; $('#retry-bookings')?.addEventListener('click', loadMyBookings); }
  }
  async function loadProfile() {
    try { const user = await request('/api/Customer/me'); const form = $('#profile-form'); for (const key of ['fullName', 'email', 'phoneNumber', 'nationalCode']) form.elements[key].value = user[key] || ''; }
    catch (error) { if (!isAdmin()) showToast(`اطلاعات حساب: ${error.message}`, true); }
  }
  function showAuth(mode = 'login') { setAuthMode(mode); $('#auth-dialog').showModal(); }
  function setAuthMode(mode) {
    state.authMode = mode; $('#auth-title').textContent = mode === 'login' ? 'ورود به حساب' : 'ساخت حساب کاربری';
    $$('[data-auth-mode]').forEach(button => button.classList.toggle('active', button.dataset.authMode === mode));
    $$('.register-only').forEach(el => el.classList.toggle('hidden', mode !== 'register'));
    $('#auth-form').elements.password.autocomplete = mode === 'login' ? 'current-password' : 'new-password';
    $('#auth-form').querySelector('button[type=submit]').textContent = mode === 'login' ? 'ورود' : 'ساخت حساب و ادامه';
    ['email', 'phoneNumber', 'nationalCode'].forEach(key => { $('#auth-form').elements[key].required = mode === 'register'; });
  }
  async function submitAuth(event) {
    event.preventDefault(); const data = formObject(event.currentTarget); const mode = state.authMode;
    try {
      const result = await request(`/api/Auth/${mode}`, { method: 'POST', body: mode === 'login' ? { userName: data.username, password: data.password } : { username: data.username, password: data.password, email: data.email, phoneNumber: data.phoneNumber, nationalCode: data.nationalCode } });
      saveSession({ accessToken: result.accessToken, refreshToken: result.refreshToken || '', expiresAt: result.expiresAt });
      $('#auth-dialog').close(); event.currentTarget.reset(); showToast(mode === 'login' ? 'با موفقیت وارد شدید.' : 'حساب شما ساخته شد. خوش آمدید!');
      updateAuthUI();
      if (document.body.dataset.page === 'account') { loadMyBookings(); loadProfile(); }
      if (document.body.dataset.page === 'admin' && isAdmin()) loadAdmin();
    } catch (error) { showToast(error.message, true); }
  }
  async function submitProfile(event) {
    event.preventDefault();
    try { const updated = await request('/api/Customer/me', { method: 'PUT', body: formObject(event.currentTarget) }); if (updated?.fullName) event.currentTarget.elements.fullName.value = updated.fullName; showToast('اطلاعات حساب ذخیره شد.'); }
    catch (error) { showToast(error.message, true); }
  }
  async function submitHotel(event) {
    event.preventDefault(); const data = formObject(event.currentTarget); data.starRating = Number(data.starRating);
    try { await request('/api/Hotel', { method: 'POST', body: data }); event.currentTarget.reset(); event.currentTarget.elements.country.value = 'ایران'; showToast('هتل با موفقیت ثبت شد.'); await loadHotels(); }
    catch (error) { showToast(error.message, true); }
  }
  async function submitRoom(event) {
    event.preventDefault(); const data = formObject(event.currentTarget);
    Object.assign(data, { hotelId: Number(data.hotelId), roomNumber: Number(data.roomNumber), capacity: Number(data.capacity), pricePerNight: Number(data.pricePerNight) });
    try { await request('/api/Room', { method: 'POST', body: data }); event.currentTarget.reset(); fillHotelSelect(); showToast('اتاق با موفقیت ثبت شد.'); await loadHotels(); }
    catch (error) { showToast(error.message, true); }
  }
  async function uploadImages(kind, id, input) {
    if (!input.files?.length) return;
    const data = new FormData(); [...input.files].forEach(file => data.append('images', file));
    try { await request(`/api/${kind}/${id}/images`, { method: 'POST', body: data }); input.value = ''; showToast('تصویرها بارگذاری شدند.'); await loadHotels(); }
    catch (error) { showToast(error.message, true); }
  }
  function renderAdminHotels() {
    $('#admin-hotel-list').innerHTML = state.hotels.map(h => `<article class="admin-hotel-card"><div class="admin-hotel-head"><h3>${esc(h.name)} <small>· ${esc(h.city)}</small></h3><div><label class="mini-action">افزودن تصویر <input type="file" accept="image/*" multiple data-upload-hotel="${h.id}" hidden></label> <button class="mini-action danger" data-delete-hotel="${h.id}">حذف هتل</button></div></div>${(state.rooms[h.id] || []).map(room => `<div class="admin-room-row"><span>اتاق ${Number(room.roomNumber).toLocaleString('fa-IR')} · ${esc(room.roomType)} · ${price(room.pricePerNight)} · <span class="status-pill">${esc(room.status)}</span></span><div class="admin-room-actions"><label class="mini-action">تصویر اتاق<input type="file" accept="image/*" multiple data-upload-room="${room.id}" hidden></label><button class="mini-action" data-room-status="${room.id}" data-action="${String(room.status).toLowerCase() === 'maintenance' ? 'available' : 'maintenance'}">${String(room.status).toLowerCase() === 'maintenance' ? 'فعال‌کردن' : 'تعمیرات'}</button><button class="mini-action danger" data-delete-room="${room.id}">حذف اتاق</button></div></div>`).join('') || '<div class="admin-room-row">اتاقی ثبت نشده است.</div>'}</article>`).join('') || '<div class="empty-state">برای شروع، یک هتل ثبت کنید.</div>';
    $$('[data-upload-hotel]').forEach(input => input.addEventListener('change', () => uploadImages('Hotel', input.dataset.uploadHotel, input)));
    $$('[data-upload-room]').forEach(input => input.addEventListener('change', () => uploadImages('Room', input.dataset.uploadRoom, input)));
    $$('[data-room-status]').forEach(button => button.addEventListener('click', async () => { try { await request(`/api/Room/${button.dataset.roomStatus}/${button.dataset.action}`, { method: 'PUT' }); showToast('وضعیت اتاق به‌روز شد.'); await loadHotels(); } catch (error) { showToast(error.message, true); } }));
    $$('[data-delete-room]').forEach(button => button.addEventListener('click', async () => { if (!confirm('این اتاق حذف شود؟')) return; try { await request(`/api/Room/${button.dataset.deleteRoom}`, { method: 'DELETE' }); showToast('اتاق حذف شد.'); await loadHotels(); } catch (error) { showToast(error.message, true); } }));
    $$('[data-delete-hotel]').forEach(button => button.addEventListener('click', async () => { if (!confirm('این هتل حذف شود؟')) return; try { await request(`/api/Hotel/${button.dataset.deleteHotel}`, { method: 'DELETE' }); showToast('هتل حذف شد.'); await loadHotels(); } catch (error) { showToast(error.message, true); } }));
  }
  async function loadAdminBookings() {
    const rows = $('#admin-booking-rows'); rows.innerHTML = '<tr><td colspan="6">در حال دریافت رزروها…</td></tr>';
    try {
      const bookings = await request('/api/Booking/get-all') || [];
      rows.innerHTML = bookings.map(b => {
        const next = b.status === 'Pending' ? ['confirm', 'تأیید و قطعی‌کردن رزرو'] : b.status === 'Confirmed' ? ['check-in', 'ثبت ورود'] : b.status === 'CheckedIn' ? ['check-out', 'ثبت خروج'] : null;
        return `<tr><td>${esc(b.customerName)}<small>${esc(b.customerEmail)}</small></td><td>${esc(b.hotelName)}<small>اتاق ${Number(b.roomNumber).toLocaleString('fa-IR')}</small></td><td>${dateLabel(b.checkInDate)}<small>تا ${dateLabel(b.checkOutDate)}</small></td><td>${price(b.totalPrice)}</td><td><span class="status-pill ${statusClass(b.status)}">${esc(bookingStatuses[b.status] || b.status)}</span></td><td>${next ? `<button class="mini-action" data-booking-action="${next[0]}" data-id="${b.id}">${next[1]}</button>` : ''}${['Pending', 'Confirmed'].includes(b.status) ? `<button class="mini-action danger" data-booking-action="cancel" data-id="${b.id}">لغو</button>` : ''}</td></tr>`;
      }).join('') || '<tr><td colspan="6">رزروی ثبت نشده است.</td></tr>';
      $$('[data-booking-action]', rows).forEach(button => button.addEventListener('click', async () => { const action = button.dataset.bookingAction; const path = action === 'cancel' ? `/api/Booking/${button.dataset.id}` : `/api/Booking/${button.dataset.id}/${action}`; try { await request(path, { method: action === 'cancel' ? 'DELETE' : 'PUT' }); showToast('وضعیت رزرو تغییر کرد.'); loadAdminBookings(); } catch (error) { showToast(error.message, true); } }));
    } catch (error) { rows.innerHTML = `<tr><td colspan="6">${esc(error.message)}</td></tr>`; }
  }
  async function loadAdmin() { await loadHotels(); await loadAdminBookings(); }
  function setupEvents() {
    $('#auth-button')?.addEventListener('click', () => showAuth());
    $('[data-open-auth]')?.addEventListener('click', () => showAuth());
    $('#logout-button')?.addEventListener('click', async () => { try { if (session.refreshToken) await request('/api/Auth/signout', { method: 'POST', body: { refreshToken: session.refreshToken } }); } catch { /* Local sign-out still succeeds when the network is unavailable. */ } clearSession(); showToast('از حساب خارج شدید.'); });
    $('#auth-form')?.addEventListener('submit', submitAuth); $$('[data-auth-mode]').forEach(button => button.addEventListener('click', () => setAuthMode(button.dataset.authMode))); $$('[data-close]').forEach(button => button.addEventListener('click', () => button.closest('dialog').close()));
    $('#profile-form')?.addEventListener('submit', submitProfile); $('#hotel-form')?.addEventListener('submit', submitHotel); $('#room-form')?.addEventListener('submit', submitRoom); $('#refresh-bookings')?.addEventListener('click', loadMyBookings); $('#admin-refresh')?.addEventListener('click', loadAdmin);
    $('#hotel-filter')?.addEventListener('input', renderHotels); $('#star-filter')?.addEventListener('change', renderHotels);
    $('#search-button')?.addEventListener('click', () => { const city = $('#search-city').value.trim(); $('#hotel-filter').value = city; state.dates.checkIn = $('#search-checkin').value; state.dates.checkOut = $('#search-checkout').value; renderHotels(); document.querySelector('#hotels').scrollIntoView({ behavior: 'smooth' }); });
    $('#menu-toggle')?.addEventListener('click', () => $('#main-nav').classList.toggle('open')); $$('#main-nav a').forEach(link => link.addEventListener('click', () => $('#main-nav')?.classList.remove('open')));
    $$('.admin-tab').forEach(button => button.addEventListener('click', () => { $$('.admin-tab').forEach(b => b.classList.toggle('active', b === button)); $$('.admin-pane').forEach(pane => pane.classList.toggle('hidden', pane.id !== button.dataset.tab)); if (button.dataset.tab === 'admin-bookings') loadAdminBookings(); }));
    ['#search-checkin', '#search-checkout'].forEach(selector => $(selector)?.addEventListener('change', () => { state.dates.checkIn = $('#search-checkin').value; state.dates.checkOut = $('#search-checkout').value; }));
    $('#check-room-dates')?.addEventListener('click', () => searchAvailableRooms(Number(new URLSearchParams(location.search).get('id'))));
    $('#room-checkin')?.addEventListener('change', () => { const date = new Date(`${$('#room-checkin').value}T00:00:00`); date.setDate(date.getDate() + 1); const min = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`; $('#room-checkout').min = min; if ($('#room-checkout').value <= $('#room-checkin').value) $('#room-checkout').value = ''; });
    document.addEventListener('click', event => {
      const thumb = event.target.closest('[data-gallery-image]');
      if (!thumb) return;
      const main = document.querySelector(`[data-gallery-target="${thumb.dataset.galleryGroup}"]`);
      if (main) main.src = thumb.dataset.galleryImage;
      $$(`[data-gallery-group="${thumb.dataset.galleryGroup}"]`).forEach(item => item.classList.toggle('active', item === thumb));
    });
  }
  setupEvents(); updateAuthUI();
  const page = document.body.dataset.page;
  if (page === 'home') { setDateDefaults(); loadHotels(); }
  if (page === 'account' && isLoggedIn()) { loadMyBookings(); loadProfile(); }
  if (page === 'admin' && isAdmin()) loadAdmin();
  if (page === 'hotel') { const hotelId = Number(new URLSearchParams(location.search).get('id')); loadHotels().then(() => openRooms(hotelId)); }
  if (page === 'room') { const params = new URLSearchParams(location.search); loadRoomDetails(Number(params.get('hotelId')), Number(params.get('roomId'))); }
})();
