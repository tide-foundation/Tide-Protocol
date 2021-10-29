import { Vue, Options } from "vue-class-component";
import { inject } from "vue";
import { BUS_KEY, SET_LOADING_KEY, SHOW_ERROR_KEY } from "@/assets/ts/Constants";
import TideInput from "@/components/Tide-Input.vue";
import router from "@/router/router";
import mainStore from "@/store/mainStore";

@Options({
  components: {
    TideInput,
  },
})
export default class Base extends Vue {
  mainStore = mainStore;
  router = router;
  bus = inject(BUS_KEY) as IBus;

  setLoading = (on: boolean) => this.bus.trigger(SET_LOADING_KEY, on);

  showAlert = (type: AlertType, msg: string) => this.bus.trigger(SHOW_ERROR_KEY, { type, msg } as Alert);
}
