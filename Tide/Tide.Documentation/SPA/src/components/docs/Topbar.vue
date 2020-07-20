<template>
  <div id="topbar">
    <div id="topbar-content">
      <div id="search-box">
        <i class="fal fa-search"></i>
        <input type="text" placeholder="Search..." />
      </div>
      <div id="btn-box"><button>Tide Button</button></div>
    </div>
  </div>
</template>

<script>
export default {
  name: "topbar",
};
</script>

<style scoped lang="scss">
#topbar {
  height: 65px;
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: flex-end;

  #topbar-content {
    width: calc(100% - 65px);
    display: flex;
    flex-direction: row;
    border-bottom: 1px solid #f5f5f5;
  }

  #search-box {
    height: 65px;
    width: 100%;
    position: relative;

    i {
      position: absolute;
      left: 5px;
      top: 24px;
      color: #c8ccd1;
      font-size: 15px;
    }

    input {
      margin-top: -1px;
      width: calc(100% - 65px);
      height: calc(100% - 3px);
      padding-left: 40px;
      border: 0px;
      font-size: 16px;
      border-radius: 0px;

      &:focus {
        outline-color: #c2d6dc;
      }

      &::placeholder {
        color: #c8ccd1;
      }
    }
  }

  #btn-box {
    width: 110px;
    display: flex;
    justify-content: flex-end;
    align-items: center;
    button {
      height: 35px;
      padding: 0 12px;

      background: #26a6d4;
      border: 0px;
      color: white;
      border-radius: 4px;
      cursor: pointer;
    }
  }
}
</style>
