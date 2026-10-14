const checkoutForm = document.querySelector('#checkoutForm');
const cartJsonInput = document.querySelector('#cartJson');
const checkoutItems = document.querySelector('#checkoutItems');
const checkoutEmpty = document.querySelector('#checkoutEmpty');
const checkoutTotals = document.querySelector('#checkoutTotals');
const checkoutSubtotal = document.querySelector('#checkoutSubtotal');
const checkoutShipping = document.querySelector('#checkoutShipping');
const checkoutTotal = document.querySelector('#checkoutTotal');
const phoneInput = document.querySelector('#Phone');
const productCatalog = new Map(
  JSON.parse(document.querySelector('#nexaCatalog').textContent)
    .map(product => [product.name, product])
);
const money = new Intl.NumberFormat('es-PE', {
  style: 'currency',
  currency: 'PEN',
  maximumFractionDigits: 0
});

function readCart() {
  try {
    const saved = JSON.parse(localStorage.getItem('nexa-cart') || '{}');
    return Object.entries(saved)
      .filter(([name, quantity]) =>
        productCatalog.has(name) && Number.isInteger(quantity) && quantity > 0
      )
      .map(([name, quantity]) => ({ name, quantity }));
  } catch {
    return [];
  }
}

function renderCheckout() {
  const cart = readCart();
  cartJsonInput.value = JSON.stringify(cart);
  checkoutItems.replaceChildren();
  checkoutEmpty.hidden = cart.length > 0;
  checkoutTotals.hidden = cart.length === 0;

  if (cart.length === 0) {
    checkoutItems.hidden = true;
    return;
  }

  checkoutItems.hidden = false;
  let subtotal = 0;

  cart.forEach(item => {
    const product = productCatalog.get(item.name);
    const lineTotal = product.price * item.quantity;
    subtotal += lineTotal;

    const row = document.createElement('article');
    row.className = 'checkout-line';
    const art = document.createElement('div');
    art.className = 'checkout-art ' + product.artClass;
    art.setAttribute('aria-hidden', 'true');

    const details = document.createElement('div');
    details.className = 'checkout-product';
    const name = document.createElement('strong');
    name.textContent = product.name;
    const quantity = document.createElement('small');
    quantity.textContent = item.quantity + (item.quantity === 1 ? ' unidad · ' : ' unidades · ') + money.format(product.price);
    details.append(name, quantity);

    const total = document.createElement('strong');
    total.textContent = money.format(lineTotal);
    row.append(art, details, total);
    checkoutItems.append(row);
  });

  const shipping = subtotal >= 250 ? 0 : 12;
  checkoutSubtotal.textContent = money.format(subtotal);
  checkoutShipping.textContent = shipping === 0 ? 'Gratis' : money.format(shipping);
  checkoutTotal.textContent = money.format(subtotal + shipping);
}

checkoutForm.addEventListener('submit', () => {
  cartJsonInput.value = JSON.stringify(readCart());
});

phoneInput.addEventListener('input', () => {
  phoneInput.value = phoneInput.value.replace(/\D/g, '').slice(0, 9);
});

window.addEventListener('storage', event => {
  if (event.key === 'nexa-cart') renderCheckout();
});

renderCheckout();
