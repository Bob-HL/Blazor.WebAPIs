window['__loadJsCssFile'] = (filename, fileType): Promise<string> => {
    let fileRef: HTMLScriptElement | HTMLLinkElement | undefined = undefined;

    if (fileType == "js") {
        fileRef = document.createElement('script')
        fileRef.setAttribute("type", "text/javascript")
        fileRef.setAttribute("src", filename)
    }
    else if (fileType == "css") {
        fileRef = document.createElement("link")
        fileRef.setAttribute("rel", "stylesheet")
        fileRef.setAttribute("type", "text/css")
        fileRef.setAttribute("href", filename)
    }

    return new Promise((resolve, reject) => {
        if (fileRef != undefined) {
            fileRef.onload = () => {
                resolve('');
            };
            fileRef.onerror = (e, s, l, n, error?: Error) => {
                reject(error?.message);
            };
            document.body.appendChild(fileRef);
        }
        else {
            reject(`File type ${fileType} is not supported.`);
        }
    });
}
