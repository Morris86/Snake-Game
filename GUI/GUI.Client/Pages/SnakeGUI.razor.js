// Store the instance globally if you need to reference it elsewhere
export function initRenderJS(dotNetHelper) {
    // Attach keydown event listener
    document.addEventListener('keydown', (event) => {
        // Log key press for debugging
        console.log('Key pressed:', event.key);

        // Invoke C# method asynchronously
        dotNetHelper.invokeMethodAsync('HandleKeyPress', event.key);
    });
}

