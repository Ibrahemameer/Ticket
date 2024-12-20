window.showSaveFilePicker = async (options) => {
    try {
        const handle = await window.showSaveFilePicker({
            suggestedName: options.suggestedName,
            types: [{
                description: 'Files',
                accept: { '*/*': ['.txt', '.pdf', '.doc', '.docx'] }
            }]
        });
        return handle.name;
    } catch (err) {
        console.error('Save file picker error:', err);
        return null;
    }
};
