const filters = document.querySelectorAll('.filter');
const products = document.querySelectorAll('.product-card');
const emptyState = document.querySelector('#emptyState');
const cartCount = document.querySelector('#cartCount');
const toast = document.querySelector('#toast');
const searchPanel = document.querySelector('#searchPanel');
const searchInput = document.querySelector('#searchInput');
const cartOverlay = document.querySelector('#cartOverlay');
const cartDrawer = document.querySelector('#cartDrawer');
const cartContent = document.querySelector('#cartContent');
const cartEmpty = document.querySelector('#cartEmpty');
const cartFooter = document.querySelector('#cartFooter');
const cartSubtotal = document.querySelector('#cartSubtotal');
const cartItemLabel = document.querySelector('#cartItemLabel');
const cartProducts = new Map([...products].map(product => {
  const button = product.querySelector('.add-button');
  return [product.dataset.name, Number(button.dataset.price)];
}));
const productArtClasses = new Map([...products].map(product => [
  product.dataset.name,
  product.querySelector('.product-art').classList[1]
]));
const money = new Intl.NumberFormat('es-PE', { style: 'currency', currency: 'PEN', maximumFractionDigits: 0 });
let cart = loadCart();
let toastTimer;
let lastFocusedElement;

function loadCart() {
  try {
    const saved = JSON.parse(localStorage.getItem('nexa-cart') || '{}');
    return Object.fromEntries(
      Object.entries(saved)
        .filter(([name, quantity]) => cartProducts.has(name) && Number.isInteger(quantity) && quantity > 0)
    );
  } catch {
    return {};
  }
}

function saveCart() {
  localStorage.setItem('nexa-cart', JSON.stringify(cart));
}

function renderCart() {
  const entries = Object.entries(cart);
  const itemCount = entries.reduce((sum, [, quantity]) => sum + quantity, 0);
  const subtotal = entries.reduce((sum, [name, quantity]) => sum + cartProducts.get(name) * quantity, 0);

  cartCount.textContent = itemCount;
  cartItemLabel.textContent = `(${itemCount})`;
  cartEmpty.hidden = itemCount > 0;
  cartFooter.hidden = itemCount === 0;
  cartContent.replaceChildren();

  entries.forEach(([name, quantity]) => {
    const row = document.createElement('article');
    row.className = 'cart-item';

    const art = document.createElement('div');
    art.className = `cart-item-art ${getProductArtClass(name)}`;
    art.setAttribute('aria-hidden', 'true');

    const details = document.createElement('div');
    details.className = 'cart-item-details';

    const title = document.createElement('h3');
    title.textContent = name;

    const price = document.createElement('p');
    price.textContent = money.format(cartProducts.get(name));

    const controls = document.createElement('div');
    controls.className = 'quantity-controls';

    const decrease = document.createElement('button');
    decrease.type = 'button';
    decrease.dataset.cartAction = 'decrease';
    decrease.dataset.product = name;
    decrease.setAttribute('aria-label', `Quitar una unidad de ${name}`);
    decrease.textContent = '−';

    const amount = document.createElement('span');
    amount.textContent = quantity;
    amount.setAttribute('aria-live', 'polite');

    const increase = document.createElement('button');
    increase.type = 'button';
    increase.dataset.cartAction = 'increase';
    increase.dataset.product = name;
    increase.setAttribute('aria-label', `Agregar una unidad de ${name}`);
    increase.textContent = '+';

    controls.append(decrease, amount, increase);
    details.append(title, price, controls);

    const lineTotal = document.createElement('strong');
    lineTotal.className = 'cart-line-total';
    lineTotal.textContent = money.format(cartProducts.get(name) * quantity);

    const remove = document.createElement('button');
    remove.type = 'button';
    remove.className = 'remove-item';
    remove.dataset.cartAction = 'remove';
    remove.dataset.product = name;
    remove.setAttribute('aria-label', `Quitar ${name} del carrito`);
    remove.textContent = 'Quitar';

    row.append(art, details, lineTotal, remove);
    cartContent.append(row);
  });

  cartSubtotal.textContent = money.format(subtotal);
}

function getProductArtClass(name) {
  return productArtClasses.get(name);
}

function updateCart(name, action) {
  const quantity = cart[name] || 0;

  if (action === 'increase' && quantity >= 10) {
    showToast('Puedes agregar hasta 10 unidades por producto');
    return;
  }
  if (action === 'increase') cart[name] = quantity + 1;
  if (action === 'decrease' && quantity > 1) cart[name] = quantity - 1;
  if (action === 'decrease' || action === 'remove') {
    if (quantity <= 1 || action === 'remove') delete cart[name];
  }

  saveCart();
  renderCart();
}

function openCart() {
  lastFocusedElement = document.activeElement;
  cartOverlay.classList.add('open');
  cartOverlay.setAttribute('aria-hidden', 'false');
  document.body.classList.add('drawer-open');
  cartDrawer.focus();
}

function closeCart() {
  cartOverlay.classList.remove('open');
  cartOverlay.setAttribute('aria-hidden', 'true');
  document.body.classList.remove('drawer-open');
  lastFocusedElement?.focus();
}

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
    updateCart(button.dataset.product, 'increase');
    showToast(`${button.dataset.product} se agregó al carrito`);
  });
});

document.querySelector('#cartButton').addEventListener('click', openCart);
document.querySelector('#closeCart').addEventListener('click', closeCart);
cartOverlay.addEventListener('click', event => {
  if (event.target === cartOverlay) closeCart();
});
cartContent.addEventListener('click', event => {
  const button = event.target.closest('[data-cart-action]');
  if (button) updateCart(button.dataset.product, button.dataset.cartAction);
});
document.querySelector('#clearCart').addEventListener('click', () => {
  cart = {};
  saveCart();
  renderCart();
});
document.querySelector('#checkoutButton').addEventListener('click', () => {
  window.location.assign('/Order/Checkout');
});
document.querySelector('#browseProducts').addEventListener('click', () => {
  closeCart();
  document.querySelector('#catalogo').scrollIntoView({ behavior: 'smooth' });
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
  if (event.key === 'Escape') {
    closeSearch();
    if (cartOverlay.classList.contains('open')) closeCart();
  }
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

renderCart();
