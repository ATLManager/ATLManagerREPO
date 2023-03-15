window.onload = function () {
    var divBox = document.getElementById("myBox")

    if (screen.width >= 1366 && screen.height >= 768 && screen.width < 1920 && screen.height >= 1080) {
    // Large screen
        divBox.classList.remove("col-6");
        divBox.classList.add("col-9");
    }else{
        divBox.classList.remove("col-9");
        divBox.classList.add("col-6");          
    }
}

