document.querySelectorAll('.catalog-list').forEach((panel) => {
    const search = panel.querySelector('.index-search');
    const sort = panel.querySelector('.index-sort');
    const factionFilter = panel.querySelector('.faction-filter');
    const list = panel.querySelector('[data-index-list]');
    const categoryButtons = [...panel.querySelectorAll('[data-equipment-category]')];
    let selectedCategory = categoryButtons[0]?.dataset.equipmentCategory ?? null;
    if (!list) return;

    const collator = new Intl.Collator('es', { sensitivity: 'accent', numeric: true });
    const normalize = (value) => value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLocaleLowerCase('es');
    let clearSearchButton = null;

    // El botón se añade aquí para compartir la misma función de limpieza en todos los índices.
    if (search) {
        const wrapper = document.createElement('span');
        wrapper.className = 'index-search-wrap';
        search.before(wrapper);
        wrapper.append(search);

        clearSearchButton = document.createElement('button');
        clearSearchButton.type = 'button';
        clearSearchButton.className = 'index-search-clear';
        clearSearchButton.setAttribute('aria-label', 'Limpiar búsqueda');
        clearSearchButton.textContent = '×';
        clearSearchButton.hidden = !search.value;
        wrapper.append(clearSearchButton);
    }

    const update = () => {
        const query = normalize(search?.value ?? '');
        const rows = [...list.querySelectorAll('.catalog-row')];
        for (const row of rows) {
            const matchesCategory = !selectedCategory || row.dataset.category === selectedCategory;
            const matchesFaction = !factionFilter?.value || row.dataset.faction === factionFilter.value;
            row.hidden = !matchesCategory || !matchesFaction || !normalize(row.dataset.search ?? row.dataset.name ?? '').includes(query);
        }

        const mode = sort?.value ?? 'name-asc';
        rows.sort((a, b) => {
            let result = 0;
            if (mode === 'price-asc' || mode === 'price-desc') {
                result = Number(a.dataset.price) - Number(b.dataset.price);
                if (mode === 'price-desc') result *= -1;
            } else if (mode === 'appears') {
                // Los elementos creados por la comunidad se reconocen primero al ordenar por expansión.
                result = Number((b.dataset.original ?? '').toLowerCase() === 'true') - Number((a.dataset.original ?? '').toLowerCase() === 'true')
                    || collator.compare(a.dataset.appears ?? '', b.dataset.appears ?? '')
                    || collator.compare(a.dataset.name ?? '', b.dataset.name ?? '');
            } else {
                result = collator.compare(a.dataset.name ?? '', b.dataset.name ?? '');
                if (mode === 'name-desc') result *= -1;
            }
            return result || Number(a.dataset.id) - Number(b.dataset.id);
        });
        rows.forEach((row) => list.append(row));

        // Si el filtro oculta la ficha seleccionada, seleccionamos la primera visible para mantener índice y tarjeta sincronizados.
        const selected = rows.find((row) => row.classList.contains('selected'));
        if (selected?.hidden) rows.find((row) => !row.hidden)?.click();
    };

    search?.addEventListener('input', () => {
        if (clearSearchButton) clearSearchButton.hidden = !search.value;
        update();
    });
    clearSearchButton?.addEventListener('click', () => {
        search.value = '';
        search.dispatchEvent(new Event('input', { bubbles: true }));
        search.focus();
    });
    sort?.addEventListener('change', update);
    factionFilter?.addEventListener('change', update);
    categoryButtons.forEach((button) => button.addEventListener('click', () => {
        selectedCategory = button.dataset.equipmentCategory;
        categoryButtons.forEach((candidate) => {
            const isSelected = candidate === button;
            candidate.classList.toggle('active', isSelected);
            candidate.setAttribute('aria-pressed', String(isSelected));
        });
        update();
    }));
    update();
});

document.querySelectorAll('[data-archive-filter]').forEach((button) => {
    button.addEventListener('click', () => {
        const selection = button.dataset.archiveFilter;
        document.querySelectorAll('[data-archive-filter]').forEach((candidate) => {
            const active = candidate === button;
            candidate.classList.toggle('active', active);
            candidate.setAttribute('aria-pressed', String(active));
        });
        document.querySelectorAll('[data-archive-card]').forEach((card) => {
            card.hidden = selection !== 'all' && card.dataset.archiveCard !== selection;
        });
    });
});

// Las fichas se guardan en <template>; cambiar selección no envía otra petición ni altera el scroll del índice.
document.addEventListener('click', (event) => {
    const skill = event.target.closest('.skill-trigger');
    const loreTrigger = event.target.closest('[data-lore-trigger], .lore-trigger');
    const popupTrigger = skill ?? loreTrigger;
    if (popupTrigger) {
        event.preventDefault();
        event.stopPropagation();
        const dialog = document.querySelector('[data-skill-dialog]');
        if (dialog) {
            dialog.querySelector('[data-skill-dialog-title]').textContent = popupTrigger.dataset.skillName ?? popupTrigger.dataset.loreTitle ?? 'Crónica';
            dialog.querySelector('[data-skill-dialog-description]').textContent = popupTrigger.dataset.fullDescription ?? '';
            dialog.querySelector('[data-skill-dialog-label]').textContent = skill ? 'BESTIARIO' : 'CRÓNICAS DEL REINO';
            if (!dialog.open) dialog.showModal();
        }
        return;
    }

    // Un clic sobre la capa oscura cierra el mismo cuadro usado por habilidades, hechizos y lore.
    if (event.target.matches('[data-skill-dialog]')) event.target.close();

    const row = event.target.closest('a[data-select-record]');
    if (!row || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) return;

    const page = row.closest('.catalog-page');
    const host = page?.querySelector('[data-detail-host]');
    const template = [...(page?.querySelectorAll('template[data-detail-template]') ?? [])]
        .find(candidate => candidate.dataset.detailTemplate === row.dataset.selectRecord);
    if (!host || !template) return;

    event.preventDefault();
    host.replaceChildren(template.content.cloneNode(true));
    page.querySelectorAll('a[data-select-record]').forEach((candidate) => {
        const selected = candidate === row;
        candidate.classList.toggle('selected', selected);
        if (selected) candidate.setAttribute('aria-current', 'page');
        else candidate.removeAttribute('aria-current');
    });

    // Actualiza la dirección para que la selección se pueda compartir; replaceState no recarga ni desplaza la página.
    window.history.replaceState(null, '', row.href);
});

document.addEventListener('keydown', (event) => {
    const trigger = event.target.closest('.lore-trigger[role="button"]');
    if (trigger && (event.key === 'Enter' || event.key === ' ')) {
        event.preventDefault();
        trigger.click();
    }
});

// Cada verso se carga por su clave desde SQLite; se conserva un texto inicial si la petición no responde.
const poemCache = new Map();
const getPoem = async (key) => {
    if (!poemCache.has(key)) {
        const request = fetch(`/Poems/${encodeURIComponent(key)}`)
            .then((response) => response.ok ? response.json() : null)
            .catch(() => null);
        poemCache.set(key, request);
    }
    return poemCache.get(key);
};

const loginVerse = document.querySelector('[data-poem-key="gate"]');
if (loginVerse) {
    getPoem('gate').then((poem) => {
        if (!poem) return;
        loginVerse.querySelector('[data-login-poem-title]').textContent = poem.title;
        loginVerse.querySelector('[data-login-poem-text]').textContent = `«${poem.text}»`;
    });
}

// El tema elegido vive en el navegador de cada persona y no cambia los colores guardados en la base de datos.
const themeButton = document.querySelector('[data-theme-toggle]');
if (themeButton) {
    const themes = ['parchment', 'moonlit'];
    const applyTheme = (theme, showVerse = false) => {
        document.documentElement.dataset.theme = theme;
        themeButton.setAttribute('aria-pressed', String(theme === 'moonlit'));
        themeButton.setAttribute('aria-label', theme === 'moonlit' ? 'Tema Luz de luna activo; cambiar a Pergamino' : 'Tema Pergamino activo; cambiar a Luz de luna');
    };
    const savedTheme = localStorage.getItem('heroquest-theme');
    applyTheme(themes.includes(savedTheme) ? savedTheme : themes[0]);
    themeButton.addEventListener('click', () => {
        const next = document.documentElement.dataset.theme === themes[0] ? themes[1] : themes[0];
        localStorage.setItem('heroquest-theme', next);
        applyTheme(next, true);
    });

    const toast = document.querySelector('[data-poem-toast]');
    let toastTimer;
    const showThemePoem = async (key) => {
        const poem = await getPoem(key);
        if (!toast || !poem) return;
        toast.querySelector('[data-poem-toast-title]').textContent = poem.title;
        toast.querySelector('[data-poem-toast-text]').textContent = poem.text;
        toast.hidden = false;
        requestAnimationFrame(() => toast.classList.add('is-visible'));
        window.clearTimeout(toastTimer);
        toastTimer = window.setTimeout(() => {
            toast.classList.remove('is-visible');
            window.setTimeout(() => { toast.hidden = true; }, 850);
        }, 3300);
    };
    themeButton.addEventListener('click', () => showThemePoem(document.documentElement.dataset.theme === 'moonlit' ? 'night' : 'day'));
}

// Este tooltip flotante se coloca en <body> para que no lo recorte el área desplazable de la ficha.
const hoverTooltip = document.createElement('div');
hoverTooltip.className = 'floating-tooltip';
hoverTooltip.setAttribute('role', 'tooltip');
hoverTooltip.hidden = true;
document.body.append(hoverTooltip);
let tooltipTarget = null;

const showTooltip = (target) => {
    if (!target?.dataset.tooltip) return;
    tooltipTarget = target;
    hoverTooltip.textContent = target.dataset.tooltip;
    hoverTooltip.hidden = false;
    const bounds = target.getBoundingClientRect();
    const width = Math.min(320, window.innerWidth - 24);
    const left = Math.max(12, Math.min(bounds.left + bounds.width / 2 - width / 2, window.innerWidth - width - 12));
    hoverTooltip.style.width = `${width}px`;
    hoverTooltip.style.left = `${left}px`;
    hoverTooltip.style.top = `${bounds.top > 130 ? bounds.top - 10 : bounds.bottom + 10}px`;
    hoverTooltip.classList.toggle('below', bounds.top <= 130);
};

const hideTooltip = (target) => {
    if (target && target !== tooltipTarget) return;
    hoverTooltip.hidden = true;
    tooltipTarget = null;
};

document.addEventListener('pointerover', (event) => showTooltip(event.target.closest('.hover-tooltip')));
document.addEventListener('pointerout', (event) => {
    const target = event.target.closest('.hover-tooltip');
    if (target && !target.contains(event.relatedTarget)) hideTooltip(target);
});
document.addEventListener('focusin', (event) => showTooltip(event.target.closest('.hover-tooltip')));
document.addEventListener('focusout', (event) => hideTooltip(event.target.closest('.hover-tooltip')));
window.addEventListener('scroll', () => hideTooltip(), true);
