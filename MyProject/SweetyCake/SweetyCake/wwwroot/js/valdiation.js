document.getElementById('registrationForm').addEventListener('submit', function (event) {
    var email = document.getElementById('Email').value;
    var phoneNumber = document.getElementById('PhoneNumber').value;
    var nationalId = document.getElementById('NationalId').value;

    var emailPattern = /\S+@\S+\.\S+/;
    var phoneNumberPattern = /^\d{11}$/;
    var nationalIdPattern = /^\d{14}$/;

    var isValid = true;

    if (!emailPattern.test(email)) {
        document.getElementById('emailError').innerText = 'Please enter a valid email address.';
        isValid = false;
    } else {
        document.getElementById('emailError').innerText = '';
    }

    if (!phoneNumberPattern.test(phoneNumber)) {
        document.getElementById('phoneNumberError').innerText = 'Phone number must be 11 digits.';
        isValid = false;
    } else {
        document.getElementById('phoneNumberError').innerText = '';
    }

    if (!nationalIdPattern.test(nationalId)) {
        document.getElementById('nationalIdError').innerText = 'National ID must be 14 digits.';
        isValid = false;
    } else {
        document.getElementById('nationalIdError').innerText = '';
    }

    if (!isValid) {
        event.preventDefault();
    }
});