window.careSync = window.careSync || {};
window.careSync.toast = {
    show: function (message, title, type, icon) {
        try {
            const toast = document.createElement('div');
            toast.className = 'toast position-fixed top-0 end-0 m-3';
            toast.setAttribute('role', 'alert');
            toast.style.zIndex = '9999';
            toast.innerHTML = `\n                <div class='toast-header bg-${type} text-white'>\n                    <i class='${icon} me-2'></i>\n                    <strong class='me-auto'>${title}</strong>\n                    <button type='button' class='btn-close btn-close-white' data-bs-dismiss='toast'></button>\n                </div>\n                <div class='toast-body'>\n                    ${message}\n                </div>\n            `;
            document.body.appendChild(toast);
            const bsToast = new bootstrap.Toast(toast);
            bsToast.show();
            setTimeout(() => toast.remove(), 5000);
        } catch (err) {
            // swallow errors - toast should not break app
            console.error('Toast show failed', err);
        }
    }
};

// If any toasts were queued before this script loaded, flush them now
try {
    if (window.careSync && Array.isArray(window.careSync._toastQueue) && window.careSync._toastQueue.length > 0) {
        window.careSync._toastQueue.forEach(t => {
            try {
                window.careSync.toast.show(t.message, t.title, t.type, t.icon);
            } catch (err) {
                console.error('Failed to flush queued toast', err);
            }
        });
        window.careSync._toastQueue.length = 0;
    }
} catch (err) {
    console.error('Error flushing toast queue', err);
}
