document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-app-toast='true']").forEach((toastElement) => {
        const toast = new bootstrap.Toast(toastElement, { delay: 4000 });
        toast.show();
    });
});
