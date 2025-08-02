async function sha256hash(plainText) {
    const encoder = new TextEncoder();
    const data = encoder.encode(plainText);
    const hash = await crypto.subtle.digest('SHA-256', data);
    return Array.from(new Uint8Array(hash))
        .map(b => b.toString(16).padStart(2, '0'))
        .join('');
}

$(document).ready(function () {
    var loginJSON = {
        title: "Login",
        elements: [
            { type: "text", name: "email", title: "Please type in your email", isRequired: true },
            { type: "text", name: "password", title: "Please enter your password", isRequired: true, inputType: "password" },
        ]
    };

    Survey.surveyLocalization.locales["en"].completeText = "Login";

    var survey = new Survey.Model(loginJSON);

    survey.onComplete.add(function (sender) {
        var credentials = {
            email: sender.data.email
        };

        (async () => {
            credentials.passwordHash = await sha256hash(sender.data.password);

            $.ajax({
                url: '/api/login',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(credentials),
                xhrFields: {
                    withCredentials: true // ✅ Allow cookies to be sent/received
                }
            })
                .done(function () {
                    window.location.href = "/";
                })
                .fail(function (xhr, status, error) {
                    console.error(xhr.responseText);
                    alert("Login failed");
                });
        })();
    });

    survey.render(document.getElementById("surveyContainer"));
}); 