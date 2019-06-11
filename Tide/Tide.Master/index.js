import Tide from "tide-js"

const tide = new Tide([], "", "tide", 32);
// document.getElementById("username").value = "thrakmar@gmail.com";
// document.getElementById("password").value = "password";

const btn = document.getElementById("connect-btn");
btn.onclick = async function () {
    try {
        disableInputs(true);

        const username = document.getElementById("username").value;
        const salted = tide.hashUsername(username);
        const password = document.getElementById("password").value;
        const desc = document.getElementById("desc").value;

        await updateStatus("Creating your master account...");
        const accountResult = await tide.createMasterAccount(username, password, false);

        await updateStatus("Generating vendor keys...");
        const vendorKeys = tide.createKeys();

        await updateStatus("Creating your vendor account...");

        const createVendorResult = await tide.tideRequest("/CreateVendor", {
            account: accountResult.account,
            username: salted.username,
            publicKey: vendorKeys.pub,
            description: desc
        });
        
        await updateStatus(`Complete.`);
        await updateStatus(`<span style="font-weight: bold"> Username:</span> ${username}`);
        await updateStatus(`<span style="font-weight: bold"> public key:</span> ${accountResult.pub}`);
        await updateStatus(`<span style="font-weight: bold"> Master private key:</span> ${accountResult.priv}`);
        await updateStatus(`<span style="font-weight: bold"> Vendor public key:</span> ${vendorKeys.pub}`);
        await updateStatus(`<span style="font-weight: bold"> Vendor private key:</span> ${vendorKeys.priv}`);

    } catch (error) {
        updateStatus(error, 'error');
    } finally {
        disableInputs(false)
    }
};

function updateStatus(msg, type = 'log') {
    return new window.Promise(
        async function (resolve, reject) {
            document.getElementById("status").innerHTML += `<p style="color:${type === 'log' ? 'white' : 'red'}">${msg}</p>`;
            setTimeout(function() {
                resolve();
            }, 100);
        })
}

function disableInputs(disable) {
    const elements = document.querySelectorAll("button, input");
    for (let i = 0; i < elements.length; i++) {
        if (disable) elements[i].classList.add('disabled');
        else elements[i].classList.remove('disabled');
    }
}