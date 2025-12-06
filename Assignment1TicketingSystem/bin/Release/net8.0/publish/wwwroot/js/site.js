// ===== Form Validation =====
document.addEventListener('DOMContentLoaded', function() {
    // Bootstrap form validation
    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
});

// ===== Alert Auto-dismiss =====
document.addEventListener('DOMContentLoaded', function() {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        // Auto-dismiss alerts after 5 seconds
        const bsAlert = new bootstrap.Alert(alert);
        setTimeout(() => {
            bsAlert.close();
        }, 5000);
    });
});

// ===== Quantity Calculator =====
function calculateTotal(ticketPrice) {
    const quantityInput = document.getElementById('quantity');
    if (!quantityInput) return;

    const updateDisplay = () => {
        const quantity = parseInt(quantityInput.value) || 0;
        const subtotal = quantity * ticketPrice;
        const total = subtotal;

        const subtotalElement = document.getElementById('subtotal');
        const totalElement = document.getElementById('total');

        if (subtotalElement) subtotalElement.textContent = '$' + subtotal.toFixed(2);
        if (totalElement) totalElement.textContent = '$' + total.toFixed(2);
    };

    quantityInput.addEventListener('change', updateDisplay);
    quantityInput.addEventListener('input', updateDisplay);
    updateDisplay();
}

// ===== Tooltip Initialization =====
document.addEventListener('DOMContentLoaded', function() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(tooltipTriggerEl => {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// ===== Modal Confirmation =====
function confirmDelete(eventTitle) {
    return confirm('Are you sure you want to delete "' + eventTitle + '"? This action cannot be undone.');
}

// ===== Search Form Handler =====
document.addEventListener('DOMContentLoaded', function() {
    const searchForm = document.querySelector('form[asp-controller="Event"][asp-action="Search"]');
    if (searchForm) {
        // Add visual feedback when search is performed
        searchForm.addEventListener('submit', function() {
            const submitBtn = searchForm.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Searching...';
            }
        });
    }
});

// ===== Filter Button Group =====
document.addEventListener('DOMContentLoaded', function() {
    const filterButtons = document.querySelectorAll('.btn-group .btn');
    filterButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            // Add active state styling
            filterButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');
        });
    });
});

// ===== Price Formatter =====
function formatPrice(price) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(price);
}

// ===== Date Formatter =====
function formatDate(dateString) {
    const options = {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    };
    return new Date(dateString).toLocaleDateString('en-US', options);
}

// ===== Copy to Clipboard =====
function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(() => {
        showNotification('Copied to clipboard!', 'success');
    }).catch(err => {
        showNotification('Failed to copy', 'error');
    });
}

// ===== Show Notification =====
function showNotification(message, type = 'info') {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    const container = document.querySelector('.container');
    if (container) {
        container.insertBefore(alertDiv, container.firstChild);

        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alertDiv);
            bsAlert.close();
        }, 5000);
    }
}

// ===== Export to CSV (Future Enhancement) =====
function exportToCSV(filename, data) {
    let csv = 'data:text/csv;charset=utf-8,';
    csv += data.map(row => row.join(',')).join('\n');

    const link = document.createElement('a');
    link.setAttribute('href', encodeURI(csv));
    link.setAttribute('download', filename);
    link.click();
}

// ===== Keyboard Shortcuts =====
document.addEventListener('keydown', function(event) {
    // Ctrl+K for search
    if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
        event.preventDefault();
        const searchInput = document.querySelector('input[placeholder*="search"]');
        if (searchInput) {
            searchInput.focus();
        }
    }

    // Escape key to dismiss modals
    if (event.key === 'Escape') {
        const modals = document.querySelectorAll('.modal.show');
        modals.forEach(modal => {
            const bsModal = bootstrap.Modal.getInstance(modal);
            if (bsModal) {
                bsModal.hide();
            }
        });
    }
});

// ===== Initialize Bootstrap Components =====
document.addEventListener('DOMContentLoaded', function() {
    // Popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    const popoverList = popoverTriggerList.map(popoverTriggerEl => {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Dropdowns
    const dropdownElementList = [].slice.call(document.querySelectorAll('[data-bs-toggle="dropdown"]'));
    const dropdownList = dropdownElementList.map(dropdownToggleEl => {
        return new bootstrap.Dropdown(dropdownToggleEl);
    });
});