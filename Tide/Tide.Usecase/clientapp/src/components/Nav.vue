<template>
  <section>
    <div id="mobile-header" class="mobile-header hidden-md hidden-lg">
      <div class="contact-number">
        <span class="desktop-version hidden-xs">1-800-555-1234</span>
        <section id="mobile-nav" class="mobile-version visible-xs-inline-block pointer" title="Make a Call">
          <span :class="{ 'nav-current-menu-item': menu == 'home' }" @click="$bus.$emit('update-menu', { name: 'Home', route: '/' })">Home</span>
          <span :class="{ 'nav-current-menu-item': menu == 'property' }" @click="
              $bus.$emit('update-menu', {
                name: 'Property',
                route: '/property'
              })
            ">Property</span>
          <span :class="{ 'nav-current-menu-item': menu == 'apply' }" @click="
              $bus.$emit('update-menu', { name: 'Apply', route: '/apply' })
            ">Apply</span>
        </section>
      </div>
      <div class="mobile-header-nav">
        <ul class="user-nav">
          <li>
            <a class="login-register-link pointer">
              <i class="fa fa-sign-in"></i>
              {{ $store.getters.user == null ? "Login" : "Logout" }}
            </a>
          </li>
          <li>
            <a>
              <i class="fa fa-user pointer"></i>Profile
            </a>
          </li>
          <li>
            <a>
              <i class="fa fa-th-list pointer"></i>My Properties
            </a>
          </li>
          <li>
            <a>
              <i class="fa fa-star pointer"></i>Favorites
            </a>
          </li>
          <li>
            <a class="submit-property-link pointer">
              <i class="fa fa-plus-circle"></i>Submit Property
            </a>
          </li>
        </ul>
        <div class="social-networks header-social-nav">
          <a class="twitter pointer">
            <i class="fa fa-twitter"></i>
          </a>
          <a class="facebook pointer">
            <i class="fa fa-facebook"></i>
          </a>
          <a class="gplus pointer">
            <i class="fa fa-google-plus"></i>
          </a>
        </div>
      </div>
    </div>
    <header class="site-header header header-variation-two">
      <div class="container">
        <div class="row zero-horizontal-margin">
          <div class="col-lg-3 zero-horizontal-padding">
            <div id="site-logo" class="site-logo">
              <div class="logo-inner-wrapper hidden-md pointer">
                <a @click="
                    $bus.$emit('update-menu', { name: 'Home', route: '/' })
                  ">
                  <img style="width:151px;height:60px" src="../assets/img/FuturePlacesLogo.png" alt="Logo" />
                </a>
                <small class="tag-line hidden-xs hidden-sm">Live your dream today</small>
              </div>
            </div>
          </div>
          <div class="col-lg-9 zero-horizontal-padding hidden-xs hidden-sm">
            <div class="header-top clearfix">
              <div class="social-networks header-social-nav" :class="{
                  'social-disable':
                    $store.getters.tideEngaged || $store.getters.tideProcessing
                }">
                <a class="twitter pointer">
                  <i class="fa fa-twitter"></i>
                </a>
                <a class="facebook pointer">
                  <i class="fa fa-facebook"></i>
                </a>
                <a class="gplus pointer">
                  <i class="fa fa-google-plus"></i>
                </a>
              </div>
              <ul class="user-nav">
                <li v-if="$store.getters.user != null">
                  <a :class="{
                      'active-user-nav-item':
                        $router.currentRoute.name == 'profile'
                    }" :style="{ 'color:orange': menu == 'profile' }" class="pointer" @click="
                      $bus.$emit('update-menu', {
                        name: 'Profile',
                        route: '/profile'
                      })
                    ">
                    <i class="fa fa-user"></i>Profile
                  </a>
                </li>

                <li>
                  <a class="submit-property-link pointer" @click="clickedLogin">
                    <i class="fa fa-sign-in"></i>
                    {{ $store.getters.user == null ? "Login" : "Logout" }}
                  </a>
                </li>
              </ul>
              <div class="contact-number">
                <svg xmlns="http://www.w3.org/2000/svg" class="contacts-icon-container" width="24" height="24" viewBox="0 0 24 24">
                  <path class="contacts-icon" fill-rule="evenodd" clip-rule="evenodd" fill="#0080BC" d="M1.027 4.846l-.018.37.01.181c.068 9.565 7.003 17.42 15.919 18.48.338.075 1.253.129 1.614.129.359 0 .744-.021 1.313-.318.328-.172.448-.688.308-1.016-.227-.528-.531-.578-.87-.625-.435-.061-.905 0-1.521 0-1.859 0-3.486-.835-4.386-1.192l.002.003-.076-.034c-.387-.156-.696-.304-.924-.422-3.702-1.765-6.653-4.943-8.186-8.896-.258-.568-1.13-2.731-1.152-6.009h.003l-.022-.223c0-1.727 1.343-3.128 2.999-3.128 1.658 0 3.001 1.401 3.001 3.128 0 1.56-1.096 2.841-2.526 3.079l.001.014c-.513.046-.914.488-.914 1.033 0 .281.251 1.028.251 1.028.015 0 .131.188.119.188-.194-.539 1.669 5.201 7.021 7.849-.001.011.636.309.636.309.47.3 1.083.145 1.37-.347.09-.151.133-.32.14-.488.356-1.306 1.495-2.271 2.863-2.271 1.652 0 2.991 1.398 2.991 3.12 0 .346-.066.671-.164.981-.3.594-.412 1.21.077 1.699.769.769 1.442-.144 1.442-.144.408-.755.643-1.625.643-2.554 0-2.884-2.24-5.222-5.007-5.222-1.947 0-3.633 1.164-4.46 2.858-2.536-1.342-4.556-3.59-5.656-6.344 1.848-.769 3.154-2.647 3.154-4.849 0-2.884-2.241-5.222-5.007-5.222-2.41 0-4.422 1.777-4.897 4.144l-.091.711z" />
                </svg>
                <span class="desktop-version hidden-xs">1-800-555-1234</span>
                <a class="mobile-version visible-xs-inline-block pointer" title="Make a Call">1-800-555-1234</a>
              </div>
            </div>
            <div class="header-bottom clearfix">
              <nav id="site-main-nav" class="site-main-nav">
                <ul class="main-menu clearfix">
                  <li :class="{ 'current-menu-item': menu == 'home' }">
                    <a class="pointer" @click="
                        $bus.$emit('update-menu', { name: 'Home', route: '/' })
                      ">Home</a>
                  </li>
                  <li :class="{ 'current-menu-item': menu == 'property' }">
                    <a class="pointer" @click="
                        $bus.$emit('update-menu', {
                          name: 'Property',
                          route: '/property'
                        })
                      ">Property</a>
                  </li>
                  <li :class="{ 'current-menu-item': menu == 'apply' }">
                    <a class="pointer" @click="
                        $bus.$emit('update-menu', {
                          name: 'Apply',
                          route: '/apply'
                        })
                      ">Apply</a>
                  </li>
                  <li :class="{ 'current-menu-item': menu == 'news' }">
                    <a class="pointer" @click="
                        $bus.$emit('update-menu', {
                          name: 'News',
                          route: '/not-real'
                        })
                      ">News</a>
                  </li>
                  <li :class="{ 'current-menu-item': menu == 'pages' }">
                    <a class="pointer" @click="
                        $bus.$emit('update-menu', {
                          name: 'Pages',
                          route: '/not-real'
                        })
                      ">Pages</a>
                  </li>
                  <li :class="{ 'current-menu-item': menu == 'galleries' }">
                    <a class="pointer" @click="
                        $bus.$emit('update-menu', {
                          name: 'Galleries',
                          route: '/not-real'
                        })
                      ">Galleries</a>
                  </li>
                  <li :class="{ 'current-menu-item': menu == 'contact' }">
                    <a class="pointer" @click="
                        $bus.$emit('update-menu', {
                          name: 'Contact',
                          route: '/not-real'
                        })
                      ">Contact</a>
                  </li>
                </ul>
              </nav>
            </div>
          </div>
        </div>
      </div>
    </header>
  </section>
</template>

<script>
export default {
    data() {
        return {
            menu: "home"
        };
    },
    mounted() {
        this.$bus.$on("update-menu", data => {
            this.updateMenu(data);
        });
    },
    created() {
        this.$nextTick(() => {
            this.updateMenu({
                name: this.$router.currentRoute.name,
                route: this.$router.currentRoute.path
            });
        });
    },
    methods: {
        updateMenu(data) {
            if (data.name != null) this.menu = data.name.toLowerCase();

            this.$router.push(data.route).catch(e => {});
        },
        profileClicked() {
            if (this.$store.getters.user == null) {
                this.$bus.$emit("showLoginModal", true);
            } else {
                this.$bus.$emit("update-menu", { name: "Profile", route: "/profile" });
            }
        },
        clickedLogin() {
            if (this.$store.getters.user != null) {
                this.$store.commit("logout");
                this.$bus.$emit("update-menu", {
                    name: "Home",
                    route: "/"
                });
            } else {
                this.$bus.$emit("show-auth", true);
            }
        }
    }
};
</script>

<style lang="scss" scoped>
#mobile-nav {
    display: flex;
    flex-direction: row;
    justify-content: space-around;
    align-items: space-around;

    span {
        margin: 7px;
    }
}

.nav-current-menu-item {
    color: orange !important;
}

.social-disable {
    opacity: 0.9;
    pointer-events: none;
}

.active-user-nav-item {
    color: orange !important;
}
</style>
