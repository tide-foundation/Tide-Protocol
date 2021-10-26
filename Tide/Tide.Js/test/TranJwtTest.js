import assert from "assert";
import { C25519Key } from "cryptide";
import Guid from "../src/guid";
import TranJwt from "../src/TranJwt"

describe('TranJwtTest', function () {
    it('should sign and verify', async function () {
        const key = C25519Key.generate();

        const jwt1 = new TranJwt(new Guid());
        jwt1.sign(key);
        
        const jwt2 = TranJwt.from(jwt1.toString());
        const isValid = jwt2.verify(key.public());
    
        assert.equal(isValid, true);
    });
});
