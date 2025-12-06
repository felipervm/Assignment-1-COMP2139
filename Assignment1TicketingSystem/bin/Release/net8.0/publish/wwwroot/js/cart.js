async function addToCart(eventId, qty=1) {
    const res = await fetch('/api/cart/add', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({eventId, qty})
    });
    if (res.ok) {
        const json = await res.json();
        document.getElementById('cartBadge').innerText = json.totalItems;
        document.getElementById('cartTotal').innerText = json.totalPrice;
        if (json.remaining <= 7) {
            alert(`Only ${json.remaining} tickets left!`);
        }
    }
}
