// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function showTab(tabId)
{
    let tabs = document.querySelectorAll(".tab-content");
    let buttons = document.querySelectorAll(".tab");

    tabs.forEach(t => t.classList.remove("active"));
    buttons.forEach(b => b.classList.remove("active"));

    document.getElementById(tabId).classList.add("active");

    event.target.classList.add("active");
}