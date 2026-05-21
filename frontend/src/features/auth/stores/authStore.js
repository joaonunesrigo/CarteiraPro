import { create } from 'zustand'
import { clearAuthToken, getAuthToken, setAuthToken } from '../services/authToken'

export const useAuthStore = create((set) => ({
  token: getAuthToken(),
  usuario: null,
  carregandoSessao: true,
  setSessao: ({ token, usuario }) => {
    setAuthToken(token)
    set({ token, usuario, carregandoSessao: false })
  },
  setUsuario: (usuario) => set({ usuario, carregandoSessao: false }),
  finalizarCarregamento: () => set({ carregandoSessao: false }),
  logout: () => {
    clearAuthToken()
    set({ token: null, usuario: null, carregandoSessao: false })
  },
}))
