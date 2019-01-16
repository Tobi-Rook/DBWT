var check = function () {
    if ((document.getElementById('password').value ==
        document.getElementById('passwordVerify').value)
        && (document.getElementById('password').value != ""
            || document.getElementById('passwordVerify').value != ""))
    {
        document.getElementById('message').style.color = 'green';
        document.getElementById('message').innerHTML = '  Passwörter stimmen überein.';
    }
    else if (!(document.getElementById('password').value ==
        document.getElementById('passwordVerify').value)
        && (document.getElementById('password').value != ""
            || document.getElementById('passwordVerify').value != ""))
    {
        document.getElementById('message').style.color = 'red';
        document.getElementById('message').innerHTML = '  Passwörter stimmen nicht überein.';
    }
    else
    {
        document.getElementById('message').innerHTML = ' ';
    }
}

var checkButtonS = function () {
    if ((document.getElementById('password').value ==
        document.getElementById('passwordVerify').value)
        && (document.getElementById('password').value != "")
        && (document.getElementById('vorname').value != "")
        && (document.getElementById('nachname').value != "")
        && (document.getElementById('email').value != "")
        && (document.getElementById('nutzername').value != "")
        && (document.getElementById('passwordVerify').value != "")
        && (document.getElementById('matrikelnummer').value != "")
        && (document.getElementById('studiengang').value != ""))
    {
        document.getElementById('passwordButton').className = "btn btn-secondary linkedButton buttonColor buttonCenter textCenter fullSize";
        document.getElementById('passwordButton').disabled = false;
    }
    else
    {
        document.getElementById('passwordButton').className = "btn btn-secondary linkedButton buttonColor buttonCenter textCenter fullSize disabled";
    }
}

var checkButtonM = function () {
    if ((document.getElementById('password').value ==
        document.getElementById('passwordVerify').value)
        && (document.getElementById('password').value != "")
        && (document.getElementById('vorname').value != "")
        && (document.getElementById('nachname').value != "")
        && (document.getElementById('email').value != "")
        && (document.getElementById('nutzername').value != "")
        && (document.getElementById('passwordVerify').value != "")) {
        document.getElementById('passwordButton').className = "btn btn-secondary linkedButton buttonColor buttonCenter textCenter fullSize";
        document.getElementById('passwordButton').disabled = false;
    }
    else
    {
        document.getElementById('passwordButton').className = "btn btn-secondary linkedButton buttonColor buttonCenter textCenter fullSize disabled";
    }
}

var checkButtonG = function () {
    if ((document.getElementById('password').value ==
        document.getElementById('passwordVerify').value)
        && (document.getElementById('password').value != "")
        && (document.getElementById('vorname').value != "")
        && (document.getElementById('nachname').value != "")
        && (document.getElementById('email').value != "")
        && (document.getElementById('nutzername').value != "")
        && (document.getElementById('passwordVerify').value != "")
        && (document.getElementById('grund').value != "")) {
        document.getElementById('passwordButton').className = "btn btn-secondary linkedButton buttonColor buttonCenter textCenter fullSize";
        document.getElementById('passwordButton').disabled = false;
    }
    else
    {
        document.getElementById('passwordButton').className = "btn btn-secondary linkedButton buttonColor buttonCenter textCenter fullSize disabled";
    }
}