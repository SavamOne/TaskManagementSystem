function message(e) {
    alert(e);
}

function getInnerTextById(id) {
    return document.getElementById(id).innerHTML
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