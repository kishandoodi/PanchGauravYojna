document.addEventListener('click', function (e) {
    const btn = e.target.closest('#openVettedPopup');
    if (!btn) return;

    const gauravId = btn.getAttribute('data-gauravid') || document.getElementById('GauravId')?.value;
    if (!gauravId) return;

    ajax.doPostAjaxHtml(
        '/Budget/VettedQuestions',
        { GauravId: gauravId },
        function (response) {
            const $container = $('#vettedModalContainer');
            $container.html(response);

            // Execute inline scripts in injected partial
            $container.find('script').each(function () {
                try {
                    $.globalEval(this.innerHTML || this.textContent || '');
                } catch (ex) {
                    console.error('Error executing injected script', ex);
                }
            });

            // Show modal
            let modalEl = document.getElementById('vettedModal');
            if (modalEl) {
                let modal = new bootstrap.Modal(modalEl);
                modal.show();
            }

            // Build dynamic form if available
            if (typeof buildVettedForm === 'function') buildVettedForm();
            if (typeof loadDynamicForm === 'function') loadDynamicForm();
        }
    );
});