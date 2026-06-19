(function (global) {
    'use strict';

    const state = { options: {}, timer: 0, observer: null };

    const apiClient = () => state.options.apiClient || global.ApiClient;
    const apiUrl = path => apiClient()?.getUrl ? apiClient().getUrl(path) : path;
    const video = () => document.querySelector('#videoOsdPage video.htmlvideoplayer, video.htmlvideoplayer, video');

    async function request(path, options) {
        const client = apiClient();
        if (client?.ajax) {
            return client.ajax(Object.assign({
                url: apiUrl(path),
                dataType: 'json',
                contentType: 'application/json'
            }, options));
        }

        const response = await fetch(apiUrl(path), Object.assign({ credentials: 'same-origin' }, options));
        if (!response.ok) throw new Error('HTTP ' + response.status);
        return response.json();
    }

    function getItemId() {
        if (typeof state.options.getItemId === 'function') {
            const configured = state.options.getItemId();
            if (configured) return configured;
        }

        const ratingButton = document.querySelector(
            '#videoOsdPage .btnUserRating[data-id], .videoOsdBottom .btnUserRating[data-id]'
        );
        if (ratingButton?.dataset.id) return ratingButton.dataset.id;

        const params = new URL(global.location.href).searchParams;
        const queryId = params.get('id') || params.get('itemId');
        if (queryId) return queryId;

        const source = video()?.currentSrc || video()?.src || '';
        const match = source.match(/\/Videos\/([a-f0-9-]{32,36})(?:\/|\?|$)/i);
        return match?.[1] || null;
    }

    function getPositionSeconds() {
        if (typeof state.options.getPositionSeconds === 'function') {
            return Number(state.options.getPositionSeconds()) || 0;
        }
        return Number(video()?.currentTime) || 0;
    }

    function timestamp(value) {
        const total = Math.max(0, Math.floor(value));
        const hours = Math.floor(total / 3600);
        const minutes = Math.floor((total % 3600) / 60);
        const seconds = total % 60;
        const pad = number => String(number).padStart(2, '0');
        return hours
            ? `[${hours}:${pad(minutes)}:${pad(seconds)}]`
            : `[${pad(minutes)}:${pad(seconds)}]`;
    }

    async function save(itemId, editor, status) {
        status.textContent = 'Salvando…';
        try {
            const body = JSON.stringify({ notesText: editor.value });
            await request('MediaNotes/' + encodeURIComponent(itemId), {
                type: 'POST',
                method: 'POST',
                data: body,
                body
            });
            status.textContent = 'Salvo';
        } catch (error) {
            status.textContent = 'Erro ao salvar';
            console.error('[Media Notes Pro] Falha ao salvar', error);
        }
    }

    async function open() {
        const itemId = getItemId();
        if (!itemId) {
            alert('Media Notes Pro: não foi possível identificar o ItemId.');
            return;
        }

        let note;
        try {
            note = await request('MediaNotes/' + encodeURIComponent(itemId), {
                type: 'GET',
                method: 'GET'
            });
        } catch (error) {
            console.error('[Media Notes Pro] Falha ao carregar', error);
            alert('Não foi possível carregar as notas.');
            return;
        }

        document.querySelector('.mnp-bg')?.remove();

        const overlay = document.createElement('div');
        overlay.className = 'mnp-bg';
        overlay.innerHTML = `
            <section class="mnp-box" role="dialog" aria-modal="true" aria-label="Bloco de Notas">
                <header class="mnp-head">
                    <h2>Bloco de Notas</h2>
                    <span class="mnp-status"></span>
                    <button type="button" class="mnp-close" aria-label="Fechar">×</button>
                </header>
                <textarea class="mnp-edit" aria-label="Notas da mídia"></textarea>
                <footer class="mnp-actions">
                    <button type="button" class="mnp-save">Salvar</button>
                    <a class="mnp-export">Exportar .txt</a>
                </footer>
            </section>`;

        const editor = overlay.querySelector('.mnp-edit');
        const status = overlay.querySelector('.mnp-status');
        const existing = note.notesText || note.NotesText || '';
        editor.value = existing + (existing && !existing.endsWith('\n') ? '\n' : '') +
            timestamp(getPositionSeconds()) + ' ';

        overlay.querySelector('.mnp-save').onclick = () => save(itemId, editor, status);
        overlay.querySelector('.mnp-close').onclick = () => overlay.remove();
        overlay.querySelector('.mnp-export').href =
            apiUrl('MediaNotes/' + encodeURIComponent(itemId) + '/Export');
        overlay.onclick = event => {
            if (event.target === overlay) overlay.remove();
        };
        editor.oninput = () => {
            clearTimeout(state.timer);
            state.timer = setTimeout(() => save(itemId, editor, status), 700);
        };

        document.body.appendChild(overlay);
        editor.focus();
        editor.selectionStart = editor.selectionEnd = editor.value.length;
    }

    function installButton() {
        if (!video()) return;

        const controls = document.querySelector(
            '#videoOsdPage .videoOsdBottom-maincontrols .buttons.focuscontainer-x,' +
            '#videoOsdPage .videoOsdBottom-maincontrols .buttons,' +
            '.videoOsdBottom .buttons'
        );
        if (!controls || controls.querySelector('.btnMediaNotes')) return;

        const button = document.createElement('button');
        button.type = 'button';
        button.setAttribute('is', 'paper-icon-button-light');
        button.className = 'btnMediaNotes autoSize paper-icon-button-light mnp-btn';
        button.title = 'Notas';
        button.setAttribute('aria-label', 'Notas');
        button.innerHTML =
            '<span class="xlargePaperIconButton material-icons" aria-hidden="true">edit_note</span>';
        button.onclick = open;

        const anchor = controls.querySelector('.btnVideoOsdSettings, .btnFullscreen');
        controls.insertBefore(button, anchor || null);
        console.info('[Media Notes Pro] botão instalado para o item', getItemId());
    }

    function configure(options) {
        state.options = Object.assign({}, state.options, options || {});
        installButton();

        if (!state.observer) {
            state.observer = new MutationObserver(installButton);
            state.observer.observe(document.documentElement, { childList: true, subtree: true });
        }
    }

    global.MediaNotesPro = { configure, open, timestamp, getItemId, installButton };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => configure({}));
    } else {
        configure({});
    }
}(window));
