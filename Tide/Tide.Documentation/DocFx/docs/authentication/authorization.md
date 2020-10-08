# Authorization

Ultimately authorization for your application is entirely up to you, but Tide offers convenient tools to get you up and running.

## Using 'Tide Authorization Plugin'

Include documentation about the future Tide plugin which will automate authorization for an application.

## Manual

1. Once the user has onboarded via Tide, a [Tide Account](linktoapi) object is returned which contains a special vendor-user [AESKey](linktoapi). When you link this Tide user to your application user account, ensure you save the VUID along with the AES ley.
2. Create an endpoint on your backend accepting a vuid as a parameter. Fetch the valid user account (including the saved [AESKey](linktoapi)), encrypt a token using the key and return it to the user.
3. Have the user call auth.decryptToken(cipher);
4. Attach the token as an authorization header for all subsequant authorized requests.
