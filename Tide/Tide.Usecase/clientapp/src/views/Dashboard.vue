<template>
  <section>
    <pageHead>
      Dashboard
    </pageHead>
    <div id="content-wrapper" class="site-content-wrapper">
      <div class="container" id="dashboard-content">
        <adminbox />
        <section id="grouped-content">
          <div id="admin-nav">
            <div
              @click="adminPage = 'Tide'"
              class="nav-item"
              :class="{ active: adminPage == 'Tide' }"
            >
              Tide
            </div>
            <section class="divider"></section>
            <div
              @click="adminPage = 'Deals'"
              class="nav-item"
              :class="{ active: adminPage == 'Deals' }"
            >
              Deals
            </div>
            <section class="divider"></section>
            <div
              @click="adminPage = 'Fields'"
              class="nav-item"
              :class="{ active: adminPage == 'Fields' }"
            >
              Fields
            </div>
            <section class="divider"></section>
            <div
              @click="adminPage = 'Settings'"
              class="nav-item"
              :class="{ active: adminPage == 'Settings' }"
            >
              Settings
            </div>
            <section class="divider"></section>
            <div
              @click="adminPage = 'About'"
              class="nav-item"
              :class="{ active: adminPage == 'About' }"
            >
              About
            </div>
          </div>
          <section id="dashboard-body">
            <div id="sub-header">{{ getSubtitle }}</div>
            <div id="body-content">
              <transition name="slide" mode="out-in">
                <About v-if="adminPage == 'About'" :key="1" />
                <Tide v-if="adminPage == 'Tide'" :key="2" />
                <Deals v-if="adminPage == 'Deals'" :key="3" />
                <Fields v-if="adminPage == 'Fields'" :key="4" />
                <Settings v-if="adminPage == 'Settings'" :key="5" />
              </transition>
            </div>
          </section>
        </section>
      </div>
    </div>
  </section>
</template>

<script>
import transitionBox from '@/components/TransitionBox.vue'
import pageHead from '../components/PageHead.vue'
import adminbox from '../components/dashboard/AdminBox.vue'
import tideInput from '../components/TideInput.vue'
import About from '@/components/dashboard/About.vue'
import Tide from '@/components/dashboard/Tide.vue'
import Deals from '@/components/dashboard/Deals.vue'
import Fields from '@/components/dashboard/Fields.vue'
import Settings from '@/components/dashboard/Settings.vue'
export default {
  components: {
    pageHead,
    tideInput,
    adminbox,
    transitionBox,
    About,
    Tide,
    Deals,
    Fields,
    Settings
  },
  data() {
    return {
      pages: [{
        page: 'Tide',
        subtitle: 'Spend and transfer your Tide'
      }, {
        page: 'Deals',
        subtitle: 'An overview of your deals'
      }, {
        page: 'Fields',
        subtitle: 'Toggle the fields you wish to make available to potential buyers'
      }, {
        page: 'Settings',
        subtitle: 'Alter the way you interact with Tide'
      },
      {
        page: 'About',
        subtitle: 'About Tide and how to intergrate it within your life'
      }],

      adminPage: 'Tide'
    }
  },
  created() {

  },
  computed: {
    getSubtitle() {
      return this.pages.find(p => p.page == this.adminPage).subtitle;
    }
  }
}
</script>

<style lang="scss" scoped>
#content-wrapper {
  background-color: #f1f4f8 !important;
}

#dashboard-content {
  display: flex;
  flex-direction: column;
}

#grouped-content {
  margin: 15px;
  background: white;
  display: flex;
  flex-direction: column;
}

#admin-nav {
  color: #adafb2;
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  font-size: 20px;
  align-items: center;
  border: 1px solid #e7e9ed;
  border-bottom: 0px;
  border-radius: 2px;
}

.nav-item {
  height: 95px;
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  width: 100%;
}

.nav-item:hover {
  color: orange;
}

.divider {
  height: 30px;
  border-left: 1px solid #d3dce2;
}

.active {
  margin-bottom: -6px;
  color: black;
  border-bottom: 5px solid #00aaff !important;
}

#sub-header {
  height: 40px;
  background-color: #f8fafc;
  color: #7f8fa4;
  display: flex;
  align-items: center;
  padding-left: 25px;
  border-bottom: 1px solid #e7e9ed;
  user-select: none;
}

#dashboard-body {
  border: 1px solid #e7e9ed;
}

#body-content {
  padding: 10px;
}
</style>
