async function loadList() {
	const list = document.getElementById('list');
	const spinner = document.getElementById('loading');
	spinner.style.display = 'block';
	const res = await fetch('api/documents');
	const items = await res.json();
	spinner.style.display = 'none';
	list.innerHTML = items
		.map(
			(i) => `
        <div class="card">
            <strong>${i.fileName}</strong>
            <small>(${new Date(i.createdUtc).toLocaleString()})</small>
            <pre>${i.summary ?? ''}</pre>
        </div>
    `
		)
		.join('');
}

document.getElementById('uploadForm').addEventListener('submit', async (e) => {
	e.preventDefault();
	const fd = new FormData();
	const f = document.getElementById('file').files[0];
	if (!f) return;
	fd.append('file', f);
	const btn = document.getElementById('upload-btn');
	btn.disabled = true;
	const res = await fetch('/api/upload', { method: 'POST', body: fd });
	const json = await res.json();
	btn.disabled = false;
	document.getElementById('result').innerHTML = res.ok
		? `<div class="card"><h3>Summary</h3><pre>${json.summary}</pre></div>`
		: `<pre style="color:red">${JSON.stringify(json, null, 2)}</pre>`;
	loadList();
});

loadList();
