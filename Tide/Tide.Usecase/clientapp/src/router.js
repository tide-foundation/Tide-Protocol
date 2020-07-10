import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'
import Property from './views/Property.vue'
import Thanks from './views/Thanks.vue'
import NotReal from './views/NotReal.vue'
import Apply from './views/Apply.vue'
import Dashboard from './views/Dashboard.vue'
import Profile from './views/Profile.vue'
import Auth from './views/Auth.vue'
import Revert from './views/Revert.vue'
import Store from './store'

Vue.use(Router)

var router = new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [{
    path: '/',
    name: 'home',
    component: Home,
    meta: []
  },
  {
    path: '/auth',
    name: 'auth',
    component: Auth,
    meta: []
  },

  {
    path: '/property',
    name: 'property',
    component: Property,
    meta: []
  },
  {
    path: '/apply',
    name: 'apply',
    component: Apply,
    meta: []
  },
  {
    path: '/thanks',
    name: 'thanks',
    component: Thanks,
    meta: []
  },
  {
    path: '/profile',
    name: 'profile',
    component: Profile,
    meta: ['auth']
  },
  {
    path: '/dashboard',
    name: 'dashboard',
    component: Dashboard,
    meta: ['auth']
  },
  {
    path: '/not-real',
    name: 'notreal',
    component: NotReal,
    meta: []
  },
  {
    path: '/revert',
    name: 'revert',
    component: Revert,
    meta: []
  }
  ]
})

router.beforeEach(async (to, from, next) => {
  if (to.meta.includes('auth')) {
    if (Store.getters.user == null) {

      Store.commit('updateRoute', {
        action: 'route',
        value: to.path
      });
      router.app.$nextTick(() => {
        router.app.$bus.$emit('showLoginModal', true)
        router.app.$bus.$emit('show-message', 'Please login or register to continue')
      })
    }

    if (Store.getters.user != null && Store.getters.details == null) {
      router.app.$loading(false, '');
      if (Store.getters.details == null) {
        router.app.$bus.$emit('show-message', 'Please apply for a property before continuing.')
        router.push('/apply');
        return;
      }
    }
  }

  Store.commit('updateTideEngaged', false);
  router.app.$bus.$emit('route-change', to.name);
  Store.commit('updateClickedTide', false);
  next()
})


Vue.prototype.$authAction = () => {
  const route = Store.getters.route;

  Store.commit('updateRoute', null)
  if (route != null && route != '') {

    if (route.action == 'event') {
      return router.app.$bus.$emit(route.value);
    } else {
      // Otherwise action == route
      return router.push(route.value);
    }
  }
}

export default router
