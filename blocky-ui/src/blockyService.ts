import axios from "axios";

const http = axios.create({
  baseURL: "http://localhost:5200",
  headers: {
    "Content-Type": "application/json"
  }
})

export interface Status {
  hasSecret: boolean;
}

export const blockyService = {

  getStatus(): Promise<Status> {
    return http.get(`/status`).then(r => r.data)
  },

  setSecret(secret: string): Promise<any> {
    return http.post(`/secret`, {secret: secret})
  },

  getBlockedList(): Promise<[]> {
    return http.get(`/blocked-list`).then(r => r.data)
  },

  block(host: string): Promise<any> {
    return http.post(`/block`, {host: host})
  },

  unblock(host: string, secret: string): Promise<any> {
    return http.post(`/unblock`, {host: host, secret: secret})
  }
}