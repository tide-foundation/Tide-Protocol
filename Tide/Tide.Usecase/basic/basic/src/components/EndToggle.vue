<template>
  <div id="end-toggle">
    <button @click="$store.commit('UPDATE_MODE',$store.getters.mode == 'Frontend' ? 'Backend' : 'Frontend')">{{$store.getters.mode == 'Frontend' ? 'Go to backend' : 'Go to frontend'}}</button>
  </div>
</template>

<script>
export default {};
</script>

<style lang="scss" scoped>
#end-toggle {
    button {
        position: absolute;
        top: 10px;
        left: 10px;
    }
}
</style>