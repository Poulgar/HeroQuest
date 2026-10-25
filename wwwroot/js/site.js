document.querySelectorAll('.catalog-list').forEach((panel) => {
    const search = panel.querySelector('.index-search');
    const sort = panel.querySelector('.index-sort');
    const list = panel.querySelector('[data-index-list]');
    if (!list) return;

    const collator = new Intl.Collator('es', { sensitivity: 'accent', numeric: true });
    const normalize = (value) => value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLocaleLowerCase('es');

    const update = () => {
        const query = normalize(search?.value ?? '');
        const rows = [...list.querySelectorAll('.catalog-row')];
        for (const row of rows) {
            row.hidden = !normalize(row.dataset.search ?? row.dataset.name ?? '').includes(query);
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
    update();
});
