$(document).ready(function () {
    let mybutton = document.getElementById("btn-back-to-top");

    // When the user scrolls down 100px from the top, show the button
    window.onscroll = function () {
        scrollFunction();
    };

    function scrollFunction() {
        if (document.body.scrollTop > 100 || document.documentElement.scrollTop > 100) {
            mybutton.style.display = "block";
        } else {
            mybutton.style.display = "none";
        }
    }

    // When the user clicks on the button, scroll to the top
    mybutton.addEventListener("click", backToTop);

    function backToTop() {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    }
});