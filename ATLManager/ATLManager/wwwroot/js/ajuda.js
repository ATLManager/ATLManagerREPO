// Script: Sempre que o botão dentro do menu ajuda é clicado mostra ou esconde a informação do mesmo
document.getElementById('showText1').addEventListener('click', function () {
    var displayText = document.getElementById('displayText1');
    var showTextBtn = document.getElementById('showText1');

    if (displayText.style.display === 'none') {
        displayText.style.display = 'block';
        showTextBtn.classList.remove('btn-help', 'btn');
        showTextBtn.classList.add('myButton');

    } else {
        displayText.style.display = 'none';
        showTextBtn.classList.remove('myButton');
        showTextBtn.classList.add('btn-help');
    }
});
//

// Script: Sempre que o botão dentro do menu ajuda é clicado mostra ou esconde a informação do mesmo
document.getElementById('showText2').addEventListener('click', function () {
    var displayText = document.getElementById('displayText2');
    var showTextBtn = document.getElementById('showText2');

    if (displayText.style.display === 'none') {
        displayText.style.display = 'block';
        showTextBtn.classList.remove('btn-help', 'btn');
        showTextBtn.classList.add('myButton');

    } else {
        displayText.style.display = 'none';
        showTextBtn.classList.remove('myButton');
        showTextBtn.classList.add('btn-help');
    }
});
//

// Script: Sempre que o botão dentro do menu ajuda é clicado mostra ou esconde a informação do mesmo
document.getElementById('showText3').addEventListener('click', function () {
    var displayText = document.getElementById('displayText3');
    var showTextBtn = document.getElementById('showText3');

    if (displayText.style.display === 'none') {
        displayText.style.display = 'block';
        showTextBtn.classList.remove('btn-help', 'btn');
        showTextBtn.classList.add('myButton');

    } else {
        displayText.style.display = 'none';
        showTextBtn.classList.remove('myButton');
        showTextBtn.classList.add('btn-help');
    }
});
//

// Script: Sempre que o botão dentro do menu ajuda é clicado mostra ou esconde a informação do mesmo
document.getElementById('showText4').addEventListener('click', function () {
    var displayText = document.getElementById('displayText4');
    var showTextBtn = document.getElementById('showText4');

    if (displayText.style.display === 'none') {
        displayText.style.display = 'block';
        showTextBtn.classList.remove('btn-help', 'btn');
        showTextBtn.classList.add('myButton');

    } else {
        displayText.style.display = 'none';
        showTextBtn.classList.remove('myButton');
        showTextBtn.classList.add('btn-help');
    }
});
//

var btnAjuda = document.getElementById('btn-ajuda');
var menuAjuda = document.getElementById('menu-ajuda');

btnAjuda.addEventListener('click', function () {
    if (menuAjuda.style.display === 'none') {
        menuAjuda.style.display = 'block';

    } else {
        // menuAjuda.style.display = 'none';
        // Necessário para dar override ás propriedades do menu
        $("#menu-ajuda").css("right", "");
        const element = document.querySelector('#menu-ajuda');
        // Sem o codigo acima funciona bem
        element.style.right = '';

    }
});

function closeSlide() {
    $("#menu-ajuda").fadeOut();
}

// Script para abrir o menu de ajuda associado ao botão de ajuda
$(document).ready(function () {
    $('#btn-ajuda').click(function () {
        if ($('#menu-ajuda').hasClass('show')) {
            // Do nothing if the menu is already open
            $('#menu-ajuda').animate({
                right: '-100%',
                // opacity: 0
            }, 1000, function () {
                $(this).hide().removeClass('show');
                $(this).css("right", "");

            });
        } else {
            // Show the menu if it's not already open
            $('#menu-ajuda').show().addClass('show');
        }
    });
});

// Script para esconder o menu de ajuda associado ao  botão para "fechar" o menu 
$(document).ready(function () {
    $('#close-ajuda').click(function () {
        if ($('#menu-ajuda').hasClass('show')) {
            $('#menu-ajuda').animate({
                right: '-100%',
                // opacity: 0
            }, 1000, function () {
                $(this).hide().removeClass('show');
                $(this).css("right", "");

            });
        }
    });
});