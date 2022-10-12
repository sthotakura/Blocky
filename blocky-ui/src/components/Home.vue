<template>
  <div
    class="flex-col bg-white rounded-lg shadow-2xl shadow-purple-500 ml-10 mr-10 lg:w-1/3 lg:mx-auto px-3 py-2 select-none bg-purple-200">
    <div class="flex mb-3 text-purple-900">
      <div class="text-2xl font-bold ml-auto mr-2">
        Blocky
      </div>
      <div class="text-sm mr-auto mt-2">
        The Website Blocker
      </div>
    </div>
    <spinner v-if="isInitializing" text="Initializing . . ." :size="10"/>
    <div v-else>
      <div v-if="hasSecret">
        <div>
          <form @submit.prevent="onSubmit">
            <input
              type="text"
              class="border border-purple-400 rounded h-8 py-1 px-3 w-full focus:outline-none focus:ring-2 focus:ring-purple-400 focus:shadow-lg"
              v-model="toBlock"
              :disabled="isBlocking"
              placeholder="enter the website address to block (e.g. twitter.com) and then press enter"/>
          </form>
        </div>
        <div class="mt-5">
          <div class="text-xl mb-2 font-bold">Currently Blocked</div>
          <div class="flex flex-wrap">
            <div v-for="blocked in blockedList"
                 class="flex justify-between px-2 py-2 bg-purple-300 hover:shadow-2xl hover:scale-105 rounded mr-1 ml-1 mb-2 tease-linear transition-all duration-150 blocked-entry">
              <div class="shrink-0 mr-2">{{ blocked }}</div>
              <button type="button" @click="onUnblock(blocked)" :title="'Unblock ' + blocked"
                      :disabled="isBlocking || isUnblocking">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                     stroke="currentColor" class="w-6 h-6">
                  <path stroke-linecap="round" stroke-linejoin="round"
                        d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0"/>
                </svg>
              </button>
            </div>
          </div>
        </div>
        <secret-input-modal
          :is-open="isSecretInputModelOpen"
          :host="toUnblock"
          @secretInputModalCancelled="onSecretInputModelCancelled"
          @secretInputModelSubmitted="onSecretInputModelSubmitted"/>
      </div>
      <div v-else>
        <form @submit.prevent="onSubmitSecret">
          <input
            type="password"
            class="border border-purple-400 rounded h-8 py-1 px-3 w-full focus:outline-none focus:ring-2 focus:ring-purple-400 focus:shadow-lg"
            v-model="secret"
            :disabled="isSettingSecret"
            placeholder="enter a value for secret and then press enter. you will need this when unblocking"/>
        </form>
      </div>
    </div>
  </div>
  <div class="flex justify-center mt-16 cursor-default select-none">
    <div>
      <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 inline" viewBox="0 0 20 20" fill="red">
        <path fill-rule="evenodd"
              d="M3.172 5.172a4 4 0 015.656 0L10 6.343l1.172-1.171a4 4 0 115.656 5.656L10 17.657l-6.828-6.829a4 4 0 010-5.656z"
              clip-rule="evenodd"/>
      </svg>
    </div>
    <div class="ml-2 mt-1">
      Suresh Thotakura - {{ buildTimeStamp }}
    </div>
  </div>
</template>

<script>
import {defineComponent, onMounted, ref} from "vue";
import {blockyService} from "../blockyService";
import Spinner from "./Spinner.vue";
import Modal from "./Modal.vue";
import SecretInputModal from "./SecretInputModal.vue";

export default defineComponent({
  name: "Home",
  components: {SecretInputModal, Modal, Spinner},
  setup() {
    const isInitializing = ref(true)
    const isBlocking = ref(false)
    const isUnblocking = ref(false)
    const isLoadingBlockedList = ref(false)
    const isSettingSecret = ref(false)
    const isSecretInputModelOpen = ref(false)

    const hasSecret = ref(false)
    const blockedList = ref([])
    const toBlock = ref("")
    const toUnblock = ref("")
    const secret = ref("")

    const refreshBlockedList = async () => {
      isLoadingBlockedList.value = true
      blockedList.value = await blockyService.getBlockedList()
      isLoadingBlockedList.value = false
    }

    const init = async () => {
      const status = await blockyService.getStatus()
      hasSecret.value = status.hasSecret
      if (hasSecret.value) {
        await refreshBlockedList()
      }
      isInitializing.value = false
    }

    onMounted(async () => await init())

    const onSubmitSecret = async () => {
      if (isSettingSecret.value) {
        return
      }
      isSettingSecret.value = true
      await blockyService.setSecret(secret.value)
      isSettingSecret.value = false
      secret.value = ""
      await init();
    }

    const onSubmit = async () => {
      if (isBlocking.value) {
        return;
      }
      isBlocking.value = true
      await blockyService.block(toBlock.value)
      isBlocking.value = false
      toBlock.value = ""
      await refreshBlockedList()
    }

    const onUnblock = async (hostToUnblock) => {
      isUnblocking.value = true
      isSecretInputModelOpen.value = true
      toUnblock.value = hostToUnblock
      isUnblocking.value = false
    }

    const onSecretInputModelCancelled = () => {
      isSecretInputModelOpen.value = false
      toUnblock.value = ""
      isUnblocking.value = false
    }

    const onSecretInputModelSubmitted = async () => {
      isSecretInputModelOpen.value = false
      toUnblock.value = ""
      isUnblocking.value = false
      await refreshBlockedList()
    }
    
    const buildTimeStamp = BUILD_TIMESTAMP

    return {
      buildTimeStamp,
      isInitializing,
      hasSecret,
      blockedList,
      onSubmit,
      toBlock,
      isBlocking,
      isLoadingBlockedList,
      onUnblock,
      isUnblocking,
      isSettingSecret,
      onSubmitSecret,
      secret,
      isSecretInputModelOpen,
      toUnblock,
      onSecretInputModelCancelled,
      onSecretInputModelSubmitted
    }
  }
})
</script>

<style scoped>
.blocked-entry {
  flex: 1 0 calc(25% - 10px);
}
</style>