window.downloadFileFromStream = async (fileName, base64String) => {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + base64String;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}