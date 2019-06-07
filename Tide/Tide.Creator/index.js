import Tide from 'tide-js'

const tide = new Tide([],"/");

document.getElementById('submit-btn').onclick = async function () {
    try {
        const username = document.getElementById("username").value;
        const password = document.getElementById("password").value;

        logMessage('Attempting login...');
        var result = await attemptTideAuth(username, password, true); // Attempt login

        if (!result.success) {
            logMessage('No account found. Registering...');
            result = await attemptTideAuth(username, password, false);
        }

        if (!result.success) {
            return logMessage('Failed registering account.');
        }

        const vendorModel = {
            account: 
        }

        // Create the vendor
        var vendorResult = await tide.tideRequest(`/createvendor`, vendorModel)

    } catch (error) {
        logMessage(error);
    }
};

function logMessage(msg) {
    document.getElementById("logs").innerHTML += msg;
}

function attemptTideAuth(username, password, login) {
    return new Promise(async (resolve, reject) => {
        try {
            var result = tide[login ? 'getCredentials' : 'postCredentials'](username, password);
            return resolve({
                success: true,
                creds: result
            });
        } catch (error) {   
            return resolve({
                success: false,
                creds: null
            });
        }
    });
}