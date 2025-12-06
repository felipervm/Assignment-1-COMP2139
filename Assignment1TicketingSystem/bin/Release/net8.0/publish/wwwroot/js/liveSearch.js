document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('searchEvents');
    if (!searchInput) return;

    let timeout;
    searchInput.addEventListener('input', () => {
        clearTimeout(timeout);
        timeout = setTimeout(async () => {
            const q = searchInput.value;
            const res = await fetch(`/events?search=${encodeURIComponent(q)}`);
            const html = await res.text();
            document.getElementById('eventsList').innerHTML = html;
        }, 250);
    });
});
