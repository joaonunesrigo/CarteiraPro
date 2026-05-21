import { useEffect, useState } from 'react'
import { authApi } from '../services/auth.api'
import { useAuthStore } from '../stores/authStore'

export function useAuth() {
  const { token, usuario, carregandoSessao, setSessao, setUsuario, finalizarCarregamento, logout } = useAuthStore()
  const [erro, setErro] = useState(null)
  const [enviando, setEnviando] = useState(false)

  useEffect(() => {
    async function carregarSessao() {
      if (!token) {
        finalizarCarregamento()
        return
      }

      try {
        const usuarioAtual = await authApi.me()
        setUsuario(usuarioAtual)
      } catch {
        logout()
      }
    }

    carregarSessao()
  }, [token, finalizarCarregamento, setUsuario, logout])

  async function autenticar(acao, dados) {
    setErro(null)
    setEnviando(true)

    try {
      const resposta = await acao(dados)
      setSessao(resposta)
      return true
    } catch (error) {
      setErro(error.message)
      return false
    } finally {
      setEnviando(false)
    }
  }

  return {
    token,
    usuario,
    autenticado: Boolean(token && usuario),
    carregandoSessao,
    enviando,
    erro,
    login: (dados) => autenticar(authApi.login, dados),
    registrar: (dados) => autenticar(authApi.registrar, dados),
    logout,
  }
}
