// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $("#loginBtn").click(function () {
        $.ajax({
            url: "/Home/Login", // Adjust based on your controller and action
            type: "GET",
            success: function (data) {
                $("#loginBtn").html(data);
            },
            error: function () {
                alert("Error loading login page.");
            }
        });
    });
});

document.getElementById("loginBtn").addEventListener("click", function () {
    fetch("/Home/Login")
        .then(response => response.text())
        .then(html => {
            document.getElementById("loginContainer").innerHTML = html;
        })
        .catch(error => console.error("Error:", error));
});
