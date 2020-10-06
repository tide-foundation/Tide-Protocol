<template>
    <span class="auth-form">
        <p v-if="$parent.step == 0">
            Create your account to access all of
            <span style="color:#29AAFC">Future Places</span>
            great features.
        </p>
        <p v-if="$parent.step == 1">Please choose your DNS ork, or use the one provided.</p>
        <p v-if="$parent.step == 2">
            We have randomly selected 3 Ork nodes to be used in the secret sharing of your
            <span style="color:#29AAFC">Tide account</span>. Feel free to change them to your liking.
        </p>
        <p v-if="$parent.step == 3">
            We've chosen 3 random orks for your
            <span style="color:#29AAFC">Future Places</span> account. Feel free to change them to your liking.
        </p>
        <br />
        <section v-if="$parent.step == 0" key="reg0">
            <form @submit.prevent="registerButtonClicked">
                <input v-model="user.email" placeholder="Email" type="email" />
                <password @score="$parent.showScore" v-model="user.password" :toggle="true" placeholder="Password" />
                <input v-model="user.confirm" placeholder="Confirm Password" type="password" />
                <div class="s-checkbox" @click="advancedSecurity = !advancedSecurity" :class="{ checked: advancedSecurity }">
                    <div class="holder">
                        <div class="slideOne" :class="{ checked: advancedSecurity }">
                            <input type="checkbox" value="None" id="sec" :checked="advancedSecurity" />
                            <label for="sec"></label>
                        </div>
                    </div>
                    Advanced Security
                </div>

                <button :class="{ disabled: !passwordsValid }" class="gradiant-button">
                    {{ advancedSecurity ? "CONTINUE" : "SIGN UP" }} &nbsp;&nbsp;
                    <i
                        :class="{
                            'fa-arrow-right': advancedSecurity,
                            'fa-sign-in': !advancedSecurity
                        }"
                        class="fa"
                    ></i>
                </button>
            </form>
        </section>
        <section v-if="$parent.step == 1" key="reg1">
            <label for="dns-ork"></label>
            <input class="f-w" v-model="dnsOrk" id="dns-ork" placeholder="Enter your manual DNS ork" type="text" :disabled="!manualDns" />
            <div class="s-checkbox" @click="manualDns = !manualDns" :class="{ checked: manualDns }">
                <div class="holder">
                    <div class="slideOne" :class="{ checked: manualDns }">
                        <input type="checkbox" value="None" id="sec" :checked="manualDns" />
                        <label for="sec"></label>
                    </div>
                </div>
                Manual DNS Selection
            </div>
            <button :class="{ disabled: dnsOrk.length == 0 }" @click="$parent.step++" class="gradiant-button">
                CONTINUE &nbsp;&nbsp;
                <i class="fa fa-arrow-right"></i>
            </button>
        </section>
        <section v-if="$parent.step == 2" key="reg2">
            <div class="accordian">
                <div class="accordian-button" @click="showCMKorks = !showCMKorks">
                    <span>Select your Master Orks</span>
                    <i
                        class="fa"
                        :class="{
                            'fa-chevron-down': !showCMKorks,
                            'fa-chevron-up': showCMKorks
                        }"
                        aria-hidden="true"
                    ></i>
                </div>
                <div class="accordian-rows" v-if="showCMKorks">
                    <div class="accordian-row" v-for="(ork, index) in orks" :key="ork.id" @click="$set(orks, index, toggledOrk(ork, 'cmk'))" :class="{ disabled: !ork.cmk && CMKSelectedCount >= 3 }">
                        <div class="enabled-col">
                            <div class="slideOne" :class="{ checked: ork.cmk }">
                                <input type="checkbox" value="None" :id="`cmk-${ork.id}`" :checked="ork.cmk" />
                                <label :for="`cmk-${ork.id}`"></label>
                            </div>
                        </div>
                        <div class="id-col">{{ ork.id }}</div>
                        <div class="endpoint-col">{{ ork.url }}</div>
                    </div>
                </div>
            </div>

            <div id="btn-bar">
                <button class="gradiant-button back" @click="$parent.step = 0"><i class="fa fa-arrow-left"></i> &nbsp;&nbsp; BACK</button>
                <button :class="{ disabled: CMKSelectedCount < 3 }" class="gradiant-button" @click="$parent.step = 3">
                    NEXT &nbsp;&nbsp;
                    <i class="fa fa-arrow-right"></i>
                </button>
            </div>
        </section>
        <section v-if="$parent.step == 3" key="reg3">
            <div class="accordian">
                <div class="accordian-button" @click="showCVKorks = !showCVKorks">
                    <span>Select your Vendor Orks</span>
                    <i
                        class="fa"
                        :class="{
                            'fa-chevron-down': !showCVKorks,
                            'fa-chevron-up': showCVKorks
                        }"
                        aria-hidden="true"
                    ></i>
                </div>
                <div class="accordian-rows" v-if="showCVKorks">
                    <div class="accordian-row" v-for="(ork, index) in orks" :key="ork.id" @click="$set(orks, index, toggledOrk(ork, 'cvk'))" :class="{ disabled: !ork.cvk && CVKSelectedCount >= 3 }">
                        <div class="slideOne" :class="{ checked: ork.cvk }">
                            <input type="checkbox" value="None" :id="`cvk-${ork.id}`" :checked="ork.cvk" />
                            <label :for="`cvk-${ork.id}`"></label>
                        </div>
                        <div class="id-col">{{ ork.id }}</div>
                        <div class="endpoint-col">{{ ork.url }}</div>
                    </div>
                </div>
            </div>
            <div id="btn-bar">
                <button class="gradiant-button back" @click="step = 1"><i class="fa fa-arrow-left"></i> &nbsp;&nbsp; BACK</button>
                <button :class="{ disabled: CVKSelectedCount < 3 }" class="gradiant-button" @click="register">
                    SIGN UP &nbsp;&nbsp;
                    <i class="fa fa-sign-in"></i>
                </button>
            </div>
        </section>
        <a href="#" @click="$parent.changeMode('Login')">Have an account?</a>
    </span>
</template>


<script>
import Password from "vue-password-strength-meter";
export default {
    components: { Password },
    props: ["user"],
    data: function() {
        return {
            // Password
            advancedSecurity: false,

            // Ork Selection
            orks: [],
            vendorProvidedOrk: ["https://ork-0.azurewebsites.net", "https://ork-1.azurewebsites.net"],
            manualDns: false,
            dnsOrk: "",
            cmkOrkIndex: [],
            cvkOrkIndex: [],
            showCMKorks: false,
            showCVKorks: false
        };
    },
    watch: {
        manualDns: function() {
            if (this.manualDns) this.dnsOrk = "";
            else this.autoSelectDnsOrk();
        }
    },
    created() {
        this.autoSelectDnsOrk();
        this.orks = this.$orks;

        console.log(this.$parent.getTemporaryOrkList());
    },
    computed: {
        passwordsValid: function() {
            return this.$parent.passwordScore == 4 && this.user.password == this.user.confirm;
        },
        CMKSelectedCount: function() {
            var length = this.orks.filter(o => o.cmk).length;
            this.$parent.error = length < 3 ? "You must select 3 ork nodes." : "";
            return length;
        },
        CVKSelectedCount: function() {
            var length = this.orks.filter(o => o.cvk).length;
            this.$parent.error = length < 3 ? "You must select 3 ork nodes." : "";
            return length;
        }
    },
    methods: {
        toggledOrk(ork, key) {
            ork[key] = !ork[key];
            console.log(ork);
            return ork;
        },
        autoSelectDnsOrk() {
            this.dnsOrk = this.vendorProvidedOrk.sort(() => 0.5 - Math.random())[0];
        },

        registerButtonClicked() {
            if (this.advancedSecurity) {
                this.$parent.step = 1;
            } else this.register();
        },
        register() {
            if (this.user.password.length < 4) return (this.$parent.error = "Password requires at least 4 characters.");

            this.$loading(true, "Creating Tide account...");

            // Artificial wait to allow loading overlay to react
            setTimeout(async () => {
                try {
                    var signUp = await this.$tide.registerV2(this.user.email, this.user.password, this.user.email, this.$parent.getTemporaryOrkList());

                    var userData = {
                        id: signUp.vuid.toString(),
                        cvkPub: signUp.publicKey
                    };

                    var result = await this.$http.post(`${this.$tide.serverUrl}/account`, userData);

                    this.$parent.setBearer(result.data);

                    this.$loading(true, "Generating your vendor account and distributing your key fragments...");

                    this.$store.commit("storeUser", userData);

                    this.$bus.$emit("show-message", "You have registered successfully");

                    this.$loading(false, "");

                    this.$router.push("/apply");
                } catch (thrownError) {
                    console.log(thrownError);
                    this.$bus.$emit("show-error", thrownError.response != null && thrownError.response.text != null ? thrownError.response.text : thrownError);
                    this.$loading(false, "");
                }
            }, 100);
        }
    }
};
</script>

<style scoped lang="scss">
input[type="checkbox"] {
    pointer-events: none;
    visibility: hidden !important;
}

/* SLIDE ONE */
.slideOne {
    pointer-events: none;
    width: 30px;
    height: 10px;
    background: #ffa722;
    margin: 20px auto;

    border-radius: 50px;
    position: relative;

    box-shadow: inset 0px 1px 1px rgba(139, 139, 139, 0.3), 0px 1px 0rgba (110, 110, 110, 0.1);

    &.checked {
        background: rgb(66, 62, 255);
    }
}

.slideOne label {
    pointer-events: none;
    display: block;
    width: 16px;
    height: 16px;

    border-radius: 50px;

    transition: all 0.4s ease;
    cursor: pointer;
    position: absolute;
    top: -4px;
    left: -3px;

    box-shadow: 0px 2px 5px 0px rgba(0, 0, 0, 0.3);
    background: #fcfff4;

    background: linear-gradient(to bottom, #fcfff4 0%, #dfe5d7 40%, #b3bead 100%);
}

.slideOne input[type="checkbox"]:checked + label {
    pointer-events: none;
    left: 17px;
}

.s-checkbox {
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: flex-start;
    align-items: center;
    cursor: pointer;
    opacity: 0.6;
    &.checked {
        opacity: 1;
    }

    .holder {
        width: 50px;
    }
}

#advanced-checkbox {
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: flex-start;
    align-items: center;
    cursor: pointer;
    opacity: 0.6;
    &.checked {
        opacity: 1;
    }

    #holder {
        width: 50px;
    }
}

#btn-bar {
    width: 100%;
    display: flex;
    justify-content: space-between;
    button {
        width: 48%;
    }
}

.accordian {
    transition: all 0.4s;

    margin: 0px;
    margin-top: -30px;
    width: 100%;
    .accordian-button {
        height: 40px;
        cursor: pointer;
        border: 1px solid rgb(66, 62, 255);
        border-bottom: 0px solid rgb(66, 62, 255);
        border-radius: 3px 3px 0 0;
        background: rgb(66, 62, 255);
        color: white;
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding-left: 7px;
        padding-right: 7px;
    }

    .accordian-rows {
        max-height: 200px;
        overflow: scroll;
        overflow-x: hidden;

        border: 1px solid rgb(66, 62, 255);
        border-radius: 0 0 3px 3px;
        padding-bottom: -1px;
    }

    .accordian-row {
        cursor: pointer;
        width: 100%;
        height: 40px;
        display: flex;
        flex-direction: row;
        align-items: center;
        border: 1px solid rgb(66, 62, 255);
        //border-top: 0px solid rgb(66, 62, 255);
        margin-top: -1px;
        margin-left: -1px;
        margin-right: -1px;
        user-select: none;

        &:hover {
            background: rgb(243, 243, 243);
        }

        .enabled-col {
            padding-left: 7px !important;
            width: 10%;
            pointer-events: none;
        }
        .id-col {
            width: 20%;
        }
        .endpoint-col {
            width: 70%;
        }

        &.disabled {
            background: rgb(221, 221, 221);
            pointer-events: none;
            cursor: unset;
            opacity: 0.3;
        }
    }
}
</style>