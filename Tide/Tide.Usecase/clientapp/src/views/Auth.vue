<template>
  <div>
    <transitionBox>
      <loading />
    </transitionBox>
    <div id="content">
      <div id="logo">
        <img src="../assets/img/FuturePlacesLogo.png" alt="logo" />
      </div>
      <img id="deco-1" src="../assets/img/auth/deco-1.png" alt="decoration" />
      <img id="deco-2" src="../assets/img/auth/deco-2.png" alt="decoration" />
      <button id="auto-fill" @click="autoFill">Auto Fill</button>

      <div id="auth-box">
        <div id="title">
          <h1>{{ loginMode }}</h1>
          <img id="divider" src="../assets/img/divider.png" alt="divider" />
        </div>
      </div>

      <Login :user="user" v-if="loginMode == 'Login'" key="1"></Login>
      <Register :user="user" v-if="loginMode == 'Register'" key="2"></Register>
      <Forgot :user="user" v-if="loginMode == 'Forgot Password'" key="3"></Forgot>
    </div>
  </div>
</template>

<script>
import transitionBox from "@/components/TransitionBox.vue";
import Login from "@/components/Auth/Login.vue";
import loading from "@/components/Loading.vue";
import Register from "@/components/Auth/Register.vue";
import Forgot from "@/components/Auth/Forgot.vue";
export default {
    components: { Login, Register, Forgot, loading, transitionBox },
    data: function() {
        return {
            loginMode: "Login",
            passwordScore: 0,
            step: 0,
            error: " ",
            user: {
                email: ``,
                password: "",
                confirm: ""
            }
        };
    },
    created() {
        this.autoFill();
    },
    watch: {
        step: function() {
            this.error = " ";
        },
        loginMode: function() {
            this.error = " ";
            this.step = 0;
        }
    },

    methods: {
        showScore(score) {
            this.passwordScore = score;
            if (this.passwordScore < 4) this.error = "Please choose a stronger password.";
            else this.error = "";
        },
        changeMode(mode) {
            this.loginMode = mode;
        },
        setBearer(token) {
            this.$http.defaults.headers.common["Authorization"] = `Bearer ${token}`;
        },
        getTemporaryOrkList() {
            return this.$orks.filter(o => o.id < 3).map(o => o.url);
        },
        autoFill() {
            this.user = {
                //email: `${Math.floor(Math.random() * 1000000)}@gmail.com`,
                email: `357541@gmail.com`,
                password: "Ff09&QcBWEXk",
                confirm: "Ff09&QcBWEXk"
            };
        }
    }
};
</script>


<style lang="scss" scoped>
img {
    pointer-events: none;
}
#content {
    width: 100%;

    height: 100vh;
    background: white;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;

    #deco-1 {
        position: absolute;
        right: 0;
        top: 0;
        max-width: 550px;
        z-index: 0;
    }

    #deco-2 {
        position: absolute;
        left: 0;
        bottom: 0;
        max-width: 400px;
        z-index: 0;
    }

    #logo {
        position: absolute;
        top: 10px;
        left: 10px;
        z-index: 99999;
        img {
            width: 150px;
        }
    }

    #auth-box {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        color: #5b4e74;
        z-index: 999999999999999999999999;
        width: 380px;
        #title {
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;

            h1 {
                margin-bottom: 7px;

                font-weight: 900;
                font-family: "Roboto";
            }

            #divider {
                display: block;
                margin: 0 auto;
                margin-bottom: 20px;
            }

            p {
                margin: 5px;
            }
        }

        section {
            width: 100%;
            max-width: 600px;
            display: flex;
            flex-direction: column;
            text-align: center;

            input {
                height: 40px;
                border: 0px;
                border-bottom: 2px solid #e4e4e4 !important;
                margin-bottom: 25px;
                padding-left: 5px;

                &:hover {
                    border-bottom: 2px solid #0dbae8 !important;
                }
            }

            .gradiant-button {
                transition: all 0.5s;
                margin-top: 20px;
                border: 0px;
                border-radius: 2px;
                height: 50px;
                font-size: 16px;
                color: white;
                margin-bottom: 10px;
                // background: linear-gradient(142deg, rgba(0, 155, 177, 1) 0%, rgba(6, 60, 141, 1) 34%, rgba(9, 9, 121, 1) 60%, rgba(16, 0, 255, 1) 100%);
                background: #3d90df;
                i {
                    margin-right: 6px;
                }

                &:hover {
                    background: #235d94;
                }

                &.back {
                    background: #ffa722;

                    &:hover {
                        background: #bd8a00;
                    }
                }

                &.disabled {
                    background: black;
                    pointer-events: none;
                    opacity: 0.5;
                }
            }
        }
        table {
            thead {
                width: 100% !important;
            }
            width: 100% !important;
        }
    }

    .link {
        color: orange;

        &:hover {
            color: #0dbae8;
        }
    }

    .fade-enter-active,
    .fade-leave-active {
        transition: opacity 0.5s;
    }
    .fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
        opacity: 0;
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

::-webkit-scrollbar {
    width: 5px;
}

/* Track */
::-webkit-scrollbar-track {
    background: #f1f1f1;
}

/* Handle */
::-webkit-scrollbar-thumb {
    background: rgb(66, 62, 255);
}

/* Handle on hover */
::-webkit-scrollbar-thumb:hover {
    background: rgb(66, 62, 255);
}

#btn-bar {
    width: 100%;
    display: flex;
    justify-content: space-between;
    button {
        width: 48%;
    }
}

form {
    display: flex;
    flex-direction: column;
}

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

#auto-fill {
    position: absolute;
    bottom: 10px;
    left: 10px;
    border: 1px solid gray;
}
</style>
