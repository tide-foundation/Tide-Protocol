<template>
    <div>
        <h1>Forgot Password</h1>
        <div v-if="step == 0">
            <h2>Send emails</h2>
            <form @submit.prevent="sendEmails">
                <div class="form-group">
                    <label for="email">Recovery Email</label>
                    <input type="text" id="email" required v-model="recoveryEmail" />
                </div>
                <div class="form-group">
                    <button type="submit">SEND EMAILS</button>
                </div>
                <p>OR</p>
                <p class="link" @click="$parent.changeMode('Login')">Login</p>
            </form>
        </div>
        <div v-if="step == 1">
            <h2>Combine Fragments</h2>
            <form @submit.prevent="reconstruct">
                <div class="form-group" v-for="i in 3" :key="i">
                    <label :for="`email${i}`">Frag 1</label>
                    <input type="text" :id="`email${i}`" required v-model="frags[`frag${i}`]" />
                </div>
                <hr />
                <div class="form-group">
                    <label for="new-password">New Password</label>
                    <input type="text" id="new-password" required v-model="newPassword" />
                </div>
                <div class="form-group">
                    <button type="submit">CONSTRUCT</button>
                </div>
                <p>OR</p>
                <p class="link" @click="$parent.changeMode('Login')">Login</p>
            </form>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            step: 0,
            recoveryEmail: "thrakmar@gmail.com",
            frags: {
                // frag1: "",
                // frag2: "",
                // frag3: "",
                frag1: "p+trV2jYbDLnWnhQjPU/WAIBu5RoIqXnNE1sOLicQWKD9LwsU4D6462EJWZ87Q77",
                frag2: "crWGb17q/2K/f5CNMQ1ldAgIrASQrCsE7+T5oKoBnTnUnKtGG4UA1WqB6jUq/4EO",
                frag3: "jOmbx5gZYdg2oZFae9qDAgtW4LpTW6azIfTXX5oQRhAX3Ys/cGAXQ3skE8xwoUkm",
                frag4: "",
                frag5: "",
                frag6: "",
                frag7: "",
                frag8: "",
                frag9: "",
                frag10: "",
            },
            newPassword: "password",
        };
    },
    methods: {
        sendEmails() {
            if (this.recoveryEmail == "") return (this.error = "Please input your email address.");
            this.$tide.recoverV2(this.recoveryEmail, this.$store.getters.tempOrksToUse);
            this.step = 1;
        },
        async reconstruct() {
            try {
                this.$loading(true, "Changing your password...");
                var shares = `${this.frags.frag1}\n${this.frags.frag2}\n${this.frags.frag3}`;

                var user = await this.$tide.reconstructV2(this.recoveryEmail, shares, this.newPassword, this.$store.getters.tempOrksToUse);
                this.$bus.$emit("show-status", "Your password has been changed");
                this.$parent.setUser(user);
            } catch (error) {
                this.$bus.$emit("show-status", error);
            } finally {
                this.$loading(false, "");
            }
        },
    },
};
</script>

<style lang="scss" scoped>
</style>