function resolveElement(elementOrSelector) {
    if (!elementOrSelector) {
        return null;
    }

    if (typeof elementOrSelector === "string") {
        return document.querySelector(elementOrSelector);
    }

    // When called from Blazor with an ElementReference, the runtime supplies a DOM element.
    return elementOrSelector instanceof Element ? elementOrSelector : null;
}

function ensureBootstrapModal(modalElement) {
    if (!window.bootstrap || !window.bootstrap.Modal) {
        console.error("Bootstrap Modal is not available on window.bootstrap. Check script loading order.");
        return null;
    }

    return window.bootstrap.Modal.getOrCreateInstance(modalElement);
}

export function show(elementOrSelector) {
    const modalElement = resolveElement(elementOrSelector);
    if (!modalElement) {
        console.warn("careSyncModal.show: modal element not found", elementOrSelector);
        return;
    }

    const modalInstance = ensureBootstrapModal(modalElement);
    if (!modalInstance) {
        return;
    }

    modalInstance.show();
}

export function hide(elementOrSelector) {
    const modalElement = resolveElement(elementOrSelector);
    if (!modalElement) {
        console.warn("careSyncModal.hide: modal element not found", elementOrSelector);
        return;
    }

    const modalInstance = ensureBootstrapModal(modalElement);
    if (!modalInstance) {
        return;
    }

    modalInstance.hide();
}
