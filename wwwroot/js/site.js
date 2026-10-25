document.querySelectorAll('.catalog-list').forEach((panel) => {
    const search = panel.querySelector('.index-search');
    const sort = panel.querySelector('.index-sort');
    const list = panel.querySelector('[data-index-list]');
    const categoryButtons = [...panel.querySelectorAll('[data-equipment-category]')];
    let selectedCategory = categoryButtons[0]?.dataset.equipmentCategory ?? null;
    if (!list) return;

    const collator = new Intl.Collator('es', { sensitivity: 'accent', numeric: true });
    const normalize = (value) => value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLocaleLowerCase('es');

    const update = () => {
        const query = normalize(search?.value ?? '');
        const rows = [...list.querySelectorAll('.catalog-row')];
        for (const row of rows) {
            const matchesCategory = !selectedCategory || row.dataset.category === selectedCategory;
            row.hidden = !matchesCategory || !normalize(row.dataset.search ?? row.dataset.name ?? '').includes(query);
        }

        const mode = sort?.value ?? 'name-asc';
        rows.sort((a, b) => {
            let result = 0;
            if (mode === 'price-asc' || mode === 'price-desc') {
                result = Number(a.dataset.price) - Number(b.dataset.price);
                if (mode === 'price-desc') result *= -1;
            } else if (mode === 'appears') {
                result = collator.compare(a.dataset.appears ?? '', b.dataset.appears ?? '') || collator.compare(a.dataset.name ?? '', b.dataset.name ?? '');
            } else {
                result = collator.compare(a.dataset.name ?? '', b.dataset.name ?? '');
                if (mode === 'name-desc') result *= -1;
            }
            return result || Number(a.dataset.id) - Number(b.dataset.id);
        });
        rows.forEach((row) => list.append(row));
    };

    search?.addEventListener('input', update);
    sort?.addEventListener('change', update);
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

// Las fichas se guardan en <template>; cambiar selección no envía otra petición ni altera el scroll del índice.
document.addEventListener('click', (event) => {
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
