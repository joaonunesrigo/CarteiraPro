import { useState } from 'react'
import { Alert } from '../../../components/Alert'
import { Button } from '../../../components/Button'
import { Input } from '../../../components/Input'

const estadoInicial = {
  nome: '',
  email: '',
  senha: '',
}

const REGEX_EMAIL = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

export function AuthPage({ auth }) {
  const [modo, setModo] = useState('login')
  const [formulario, setFormulario] = useState(estadoInicial)
  const [errosCampos, setErrosCampos] = useState({})
  const cadastro = modo === 'cadastro'

  function alterarCampo(campo, valor) {
    setFormulario((atual) => ({ ...atual, [campo]: valor }))
    setErrosCampos((atual) => {
      if (!atual[campo]) return atual
      const proximo = { ...atual }
      delete proximo[campo]
      return proximo
    })
  }

  function alternarModo() {
    setModo(cadastro ? 'login' : 'cadastro')
    setFormulario(estadoInicial)
    setErrosCampos({})
  }

  function validarFormulario() {
    const erros = {}

    if (cadastro && !formulario.nome.trim()) {
      erros.nome = 'Informe seu nome.'
    }

    const email = formulario.email.trim()
    if (!email) {
      erros.email = 'Informe seu e-mail.'
    } else if (!REGEX_EMAIL.test(email)) {
      erros.email = 'Informe um e-mail válido.'
    }

    if (!formulario.senha) {
      erros.senha = 'Informe sua senha.'
    } else if (cadastro && formulario.senha.length < 8) {
      erros.senha = 'A senha deve ter pelo menos 8 caracteres.'
    }

    return erros
  }

  async function enviar(evento) {
    evento.preventDefault()

    const erros = validarFormulario()
    if (Object.keys(erros).length > 0) {
      setErrosCampos(erros)
      return
    }

    setErrosCampos({})
    const dados = cadastro
      ? formulario
      : { email: formulario.email, senha: formulario.senha }

    await (cadastro ? auth.registrar(dados) : auth.login(dados))
  }

  return (
    <main className="min-h-screen bg-slate-950 px-4 py-10 text-slate-100">
      <div className="mx-auto flex min-h-[calc(100vh-5rem)] max-w-5xl items-center justify-center">
        <div className="grid w-full gap-8 overflow-hidden rounded-3xl border border-slate-800 bg-slate-900/80 shadow-2xl shadow-black/30 md:grid-cols-[1fr_420px]">
          <section className="flex flex-col justify-center gap-5 bg-gradient-to-br from-violet-700/30 via-slate-900 to-slate-950 p-8 md:p-10">
            <p className="text-sm font-semibold uppercase tracking-[0.3em] text-violet-300">CarteiraPro</p>
            <div className="space-y-4">
              <h1 className="text-3xl font-bold tracking-tight text-white md:text-4xl">
                Sua carteira de investimentos, finalmente sob controle.
              </h1>
              <p className="max-w-xl text-base leading-relaxed text-slate-300">
                Importe ativos, operações e proventos direto da B3, acompanhe a evolução do patrimônio
                e descubra a rentabilidade real de cada posição em um painel feito para o investidor pessoa física.
              </p>
              <ul className="space-y-2 pt-2 text-sm text-slate-300">
                <li className="flex items-start gap-2">
                  <span aria-hidden className="mt-1 h-1.5 w-1.5 flex-none rounded-full bg-violet-400" />
                  Importação de extratos da B3 em poucos cliques
                </li>
                <li className="flex items-start gap-2">
                  <span aria-hidden className="mt-1 h-1.5 w-1.5 flex-none rounded-full bg-violet-400" />
                  Cotações atualizadas, rentabilidade e evolução patrimonial
                </li>
                <li className="flex items-start gap-2">
                  <span aria-hidden className="mt-1 h-1.5 w-1.5 flex-none rounded-full bg-violet-400" />
                  Seus dados isolados em uma conta só sua
                </li>
              </ul>
            </div>
          </section>

          <section className="p-6 md:p-8">
            <div className="mb-6">
              <h2 className="text-2xl font-semibold text-white">
                {cadastro ? 'Crie sua conta gratuita' : 'Bem-vindo de volta'}
              </h2>
              <p className="mt-1 text-sm text-slate-400">
                {cadastro
                  ? 'Leva menos de um minuto para começar a acompanhar seus investimentos.'
                  : 'Entre para continuar de onde parou e acompanhar sua carteira.'}
              </p>
            </div>

            <form noValidate className="space-y-6" onSubmit={enviar}>
              {cadastro && (
                <Input
                  rotulo="Nome"
                  name="nome"
                  value={formulario.nome}
                  onChange={(evento) => alterarCampo('nome', evento.target.value)}
                  autoComplete="name"
                  erro={errosCampos.nome}
                />
              )}

              <Input
                rotulo="E-mail"
                name="email"
                type="email"
                value={formulario.email}
                onChange={(evento) => alterarCampo('email', evento.target.value)}
                autoComplete="email"
                erro={errosCampos.email}
              />

              <Input
                rotulo="Senha"
                name="senha"
                type="password"
                value={formulario.senha}
                onChange={(evento) => alterarCampo('senha', evento.target.value)}
                autoComplete={cadastro ? 'new-password' : 'current-password'}
                erro={errosCampos.senha}
              />

              {auth.erro && <Alert variante="error">{auth.erro}</Alert>}

              <Button type="submit" className="w-full justify-center" disabled={auth.enviando}>
                {auth.enviando ? 'Aguarde...' : cadastro ? 'Criar conta' : 'Entrar'}
              </Button>
            </form>

            <button
              type="button"
              className="mt-5 w-full text-center text-sm text-violet-300 transition hover:text-violet-200"
              onClick={alternarModo}
            >
              {cadastro ? 'Já tenho conta. Entrar.' : 'Ainda não tenho conta. Criar cadastro.'}
            </button>
          </section>
        </div>
      </div>
    </main>
  )
}
