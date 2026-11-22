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

// Cada estante filtra su lista activa o desactivada en el navegador, sin recargar la página.
document.querySelectorAll('[data-library-section]').forEach((section) => {
    const search = section.querySelector('[data-library-search]');
    const clear = section.querySelector('[data-library-clear]');
    const expansion = section.querySelector('[data-library-expansion]');
    const classification = section.querySelector('[data-library-classification]');
    const sort = section.querySelector('[data-library-sort]');
    const list = section.querySelector('[data-library-list]');
    const empty = section.querySelector('[data-library-empty]');
    if (!list) return;

    const normalizeLibraryText = (value) => value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLocaleLowerCase('es');
    const collator = new Intl.Collator('es', { sensitivity: 'accent', numeric: true });
    const updateLibraryList = () => {
        const query = normalizeLibraryText(search?.value ?? '');
        const rows = [...list.querySelectorAll('[data-library-row]')];
        rows.forEach((row) => {
            const matchesText = normalizeLibraryText(row.dataset.search ?? '').includes(query);
            const matchesExpansion = !expansion?.value || (row.dataset.expansion ?? '').split(',').some(value => normalizeLibraryText(value.trim()) === normalizeLibraryText(expansion.value));
            const matchesClassification = !classification?.value || normalizeLibraryText(row.dataset.classification ?? '') === normalizeLibraryText(classification.value);
            row.hidden = !matchesText || !matchesExpansion || !matchesClassification;
        });

        rows.sort((a, b) => collator.compare(a.dataset.name ?? '', b.dataset.name ?? '') * (sort?.value === 'za' ? -1 : 1));
        rows.forEach((row) => list.append(row));
        if (empty) empty.hidden = rows.some(row => !row.hidden);
        if (clear) clear.hidden = !search?.value;
    };

    search?.addEventListener('input', updateLibraryList);
    clear?.addEventListener('click', () => {
        search.value = '';
        updateLibraryList();
        search.focus();
    });
    expansion?.addEventListener('change', updateLibraryList);
    classification?.addEventListener('change', updateLibraryList);
    sort?.addEventListener('change', updateLibraryList);
    updateLibraryList();
});

// Los selectores de biblioteca buscan mientras se escribe y guardan los valores elegidos como etiquetas.
document.querySelectorAll('[data-search-picker]').forEach((picker) => {
    const input = picker.querySelector('[data-picker-input]');
    const add = picker.querySelector('[data-picker-add]');
    const suggestionBox = picker.querySelector('[data-picker-suggestions]');
    const options = [...picker.querySelectorAll('[data-picker-option]')];
    const selectedList = picker.querySelector('[data-picker-selected]');
    const multiple = picker.dataset.multiple === 'True' || picker.dataset.multiple === 'true';
    const normalizePicker = (value) => value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLocaleLowerCase('es');

    const selectedValues = () => [...selectedList.querySelectorAll('[data-picker-item]')].map((item) => item.dataset.value);
    const refreshSuggestions = () => {
        const query = normalizePicker(input.value.trim());
        let visibleCount = 0;
        options.forEach((option) => {
            const alreadySelected = selectedValues().some((value) => normalizePicker(value) === normalizePicker(option.dataset.pickerOption));
            const matches = query && normalizePicker(option.dataset.pickerOption).includes(query) && !alreadySelected && visibleCount < 8;
            option.hidden = !matches;
            if (matches) visibleCount += 1;
        });
        suggestionBox.hidden = visibleCount === 0;
    };
    const addValue = (value) => {
        const match = options.find((option) => normalizePicker(option.dataset.pickerOption) === normalizePicker(value));
        if (!match) return;
        value = match.dataset.pickerOption;
        const current = selectedValues();
        if (!multiple) current.length = 0;
        if (!current.some((item) => normalizePicker(item) === normalizePicker(value))) current.push(value);
        selectedList.replaceChildren(...current.map((itemValue) => {
            const item = document.createElement('li');
            item.dataset.pickerItem = '';
            item.dataset.value = itemValue;
            const label = document.createElement('span');
            label.textContent = itemValue;
            const hidden = document.createElement('input');
            hidden.type = 'hidden'; hidden.name = picker.dataset.fieldName; hidden.value = itemValue;
            const remove = document.createElement('button');
            remove.type = 'button'; remove.dataset.pickerRemove = ''; remove.setAttribute('aria-label', `Quitar ${itemValue}`); remove.textContent = '×';
            remove.disabled = picker.dataset.locked === 'True' || picker.dataset.locked === 'true';
            item.append(label, hidden, remove);
            return item;
        }));
        input.value = '';
        suggestionBox.hidden = true;
        refreshSuggestions();
    };

    input.addEventListener('input', refreshSuggestions);
    add.addEventListener('click', () => {
        const query = normalizePicker(input.value.trim());
        const match = options.find((option) => normalizePicker(option.dataset.pickerOption) === query);
        const firstVisible = options.find((option) => !option.hidden);
        addValue(match?.dataset.pickerOption ?? firstVisible?.dataset.pickerOption ?? '');
    });
    suggestionBox.addEventListener('click', (event) => {
        const option = event.target.closest('[data-picker-option]');
        if (option) addValue(option.dataset.pickerOption);
    });
    selectedList.addEventListener('click', (event) => {
        const remove = event.target.closest('[data-picker-remove]');
        if (remove) { remove.closest('[data-picker-item]').remove(); refreshSuggestions(); }
    });
    input.addEventListener('keydown', (event) => {
        if (event.key === 'Enter') { event.preventDefault(); add.click(); }
    });
});

// Al editar una ficha existente, el servidor recibe una solicitud por campo para evitar cambios accidentales en el resto.
document.querySelectorAll('[data-field-editor][data-update-url]').forEach((editor) => {
    editor.addEventListener('click', async (event) => {
        const editButton = event.target.closest('[data-field-edit]');
        const saveButton = event.target.closest('[data-field-save]');
        const cancelButton = event.target.closest('[data-field-cancel]');
        const field = event.target.closest('[data-edit-field]');
        if (!field) return;

        const controls = [...field.querySelectorAll('input:not([type="hidden"]), textarea, select, [data-picker-add], [data-picker-remove]')];
        const setEditing = (editing) => {
            field.classList.toggle('is-editing', editing);
            controls.forEach((control) => {
                if (control.hasAttribute('readonly')) control.readOnly = !editing;
                else control.disabled = !editing;
            });
            field.querySelectorAll('[data-field-edit]').forEach((button) => { button.hidden = editing; });
            field.querySelectorAll('[data-field-save], [data-field-cancel]').forEach((button) => { button.hidden = !editing; });
            field.querySelector('[data-search-picker]')?.setAttribute('data-locked', String(!editing));
        };

        if (editButton) { setEditing(true); field.querySelector('input:not([type="hidden"]), textarea, [data-picker-input]')?.focus(); return; }
        if (cancelButton) { window.location.reload(); return; }
        if (!saveButton) return;

        const form = editor.closest('form');
        const formData = new FormData();
        formData.append('type', form.querySelector('[name="Type"]').value);
        formData.append('id', form.querySelector('[name="Id"]').value);
        formData.append('field', field.dataset.updateField);
        const picker = field.querySelector('[data-search-picker]');
        if (picker) {
            const values = [...picker.querySelectorAll('[data-picker-item]')].map((item) => item.dataset.value);
            if (field.dataset.updateField === 'AppearsOptions' || field.dataset.updateField === 'AbilityOptions') values.forEach((value) => formData.append('values', value));
            else formData.append('value', values[0] ?? '');
        } else {
            const input = field.querySelector('input:not([type="hidden"]), textarea, select');
            formData.append('value', input?.value ?? '');
        }
        formData.append('__RequestVerificationToken', editor.querySelector('[name="__RequestVerificationToken"]').value);
        saveButton.disabled = true;
        try {
            const response = await fetch(editor.dataset.updateUrl, { method: 'POST', body: formData, headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            if (!response.ok) throw new Error('El dato no se pudo guardar. Revisa el valor e inténtalo de nuevo.');
            setEditing(false);
        } catch (error) {
            window.alert(error.message);
        } finally {
            saveButton.disabled = false;
        }
    });
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
        }, 5500);
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
