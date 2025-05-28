// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    var toggleSidebarButton = document.getElementById("toggleSidebarButton");
    var sidebar = document.getElementById("sidebar");
    var navbar = document.querySelector(".navbar");

    toggleSidebarButton.addEventListener("click", function () {
        sidebar.classList.toggle("sidebar-open");
        navbar.classList.toggle("navbar-open");
    });
});