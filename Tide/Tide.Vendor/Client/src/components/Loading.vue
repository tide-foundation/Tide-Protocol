<template>
  <div class="loading" v-if="$store.getters.loading.active">
    <h4>{{ $store.getters.loading.text }}</h4>
  </div>
</template>

<script>
export default {};
</script>

<style scoped>
img {
    width: 50px;
    height: 50px;
    animation: spin 1s linear infinite;
    margin-bottom: 30px;
}

h4 {
    color: white;
}

.loading {
    text-align: center;
    z-index: 1000000000000;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    width: 100%;
    height: 100%;
    position: fixed;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    background-color: rgba(0, 0, 0, 0.8);
}

@keyframes spin {
    0% {
        transform: rotate(0);
    }
    100% {
        transform: rotate(-360deg);
    }
}
</style>
