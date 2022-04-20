function message(e) {
    alert(e);
}

function getInnerTextById(id) {
    return document.getElementById(id).innerHTML
}

function getValueById(id) {
    return document.getElementById(id).value
}

function set(key, value) {
    localStorage.setItem(key, value);
}

function get(key) {
    return localStorage.getItem(key);
}

function remove(key) {
    return localStorage.removeItem(key);
}

async function needToSubscribe() {
    if (!("Notification" in window)) {
        console.log("This browser does not support desktop notification");
        return false;
    }

    if (Notification.permission === "denied") {
        return false;
    }

    const worker = await navigator.serviceWorker.getRegistration();
    const existingSubscription = await worker.pushManager.getSubscription();
    return existingSubscription == null;
}

async function unsubscribeFromNotifications() {
    const worker = await navigator.serviceWorker.getRegistration();
    const existingSubscription = await worker.pushManager.getSubscription();
    if (existingSubscription) {
        await existingSubscription.unsubscribe()
        return existingSubscription.endpoint;
    }
}

async function requestNotificationSubscription(publicKey) {
    const worker = await navigator.serviceWorker.getRegistration();
    try {
        const subscription = await worker.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: publicKey
        });
        if (subscription) {
            return {
                url: subscription.endpoint,
                p256dh: arrayBufferToBase64(subscription.getKey('p256dh')),
                auth: arrayBufferToBase64(subscription.getKey('auth')),
            };
        }
    } catch (error) {
        if (error.name === 'NotAllowedError') {
            return null;
        }
        throw error;
    }
}

function arrayBufferToBase64(buffer) {
    // https://stackoverflow.com/a/9458996
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}