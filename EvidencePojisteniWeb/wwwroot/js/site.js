

// Logika pro poznámkovou lištu vývojáře - všechno pod
function toggleDevDrawer() {
    const drawer = document.getElementById("devTasksDrawer");
    drawer.classList.toggle("open");
}

// uložení úkolů
document.addEventListener("DOMContentLoaded", () => {
    const textarea = document.getElementById("devTasks");
    if (!textarea) return;

    textarea.value = localStorage.getItem("devTasks") || "";

    textarea.addEventListener("input", () => {
        localStorage.setItem("devTasks", textarea.value);
    });
});
