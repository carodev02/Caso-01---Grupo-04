const filters = document.querySelectorAll('.filter');
const products = document.querySelectorAll('.product-card');
const emptyState = document.querySelector('#emptyState');
const cartCount = document.querySelector('#cartCount');
const toast = document.querySelector('#toast');
const searchPanel = document.querySelector('#searchPanel');
const searchInput = document.querySelector('#searchInput');
let cartItems = 0;
let toastTimer;

function filterProducts(category = 'all', term = '') {
  let visible = 0;
  const normalizedTerm = term.trim().toLocaleLowerCase('es');

  products.forEach(product => {
    const matchesCategory = category === 'all' || product.dataset.category === category;
    const matchesTerm = product.dataset.name.toLocaleLowerCase('es').includes(normalizedTerm);
    const shouldShow = matchesCategory && matchesTerm;
    product.classList.toggle('hidden', !shouldShow);
    if (shouldShow) visible += 1;
  });

  emptyState.hidden = visible !== 0;
}

function showToast(message) {
  window.clearTimeout(toastTimer);
  toast.textContent = message;
  toast.classList.add('show');
  toastTimer = window.setTimeout(() => toast.classList.remove('show'), 2400);
}

filters.forEach(filter => {
  filter.addEventListener('click', () => {
    filters.forEach(item => item.classList.remove('active'));
    filter.classList.add('active');
    filterProducts(filter.dataset.filter);
  });
});

document.querySelectorAll('.add-button').forEach(button => {
  button.addEventListener('click', () => {
    cartItems += 1;
    cartCount.textContent = cartItems;
    showToast(`${button.dataset.product} se agregó al carrito`);
  });
});

document.querySelector('#cartButton').addEventListener('click', () => {
  showToast(cartItems ? `${cartItems} producto${cartItems === 1 ? '' : 's'} en tu carrito` : 'Tu carrito está vacío');
});

document.querySelector('#searchButton').addEventListener('click', () => {
  searchPanel.classList.add('open');
  searchPanel.setAttribute('aria-hidden', 'false');
  window.setTimeout(() => searchInput.focus(), 100);
});

function closeSearch() {
  searchPanel.classList.remove('open');
  searchPanel.setAttribute('aria-hidden', 'true');
  searchInput.value = '';
  filterProducts(document.querySelector('.filter.active').dataset.filter);
}

document.querySelector('#closeSearch').addEventListener('click', closeSearch);
searchPanel.addEventListener('click', event => {
  if (event.target === searchPanel) closeSearch();
});
searchInput.addEventListener('input', event => filterProducts('all', event.target.value));
document.addEventListener('keydown', event => {
  if (event.key === 'Escape') closeSearch();
});

document.querySelector('#newsletterForm').addEventListener('submit', event => {
  event.preventDefault();
  event.currentTarget.reset();
  showToast('Suscripción confirmada');
});

fetch('/estado')
  .then(response => {
    if (!response.ok) throw new Error('Servicio no disponible');
    return response.text();
  })
  .then(() => {
    document.querySelector('.status').classList.add('online');
    document.querySelector('#serviceStatus').textContent = 'Servicio operativo';
  })
  .catch(() => {
    document.querySelector('.status').classList.add('offline');
    document.querySelector('#serviceStatus').textContent = 'Servicio no disponible';
  });
