import { Alert } from '../../components/Alert'
import { Button } from '../../components/Button'
import { IconeCarteira, IconeSair } from '../../components/Icons'
import { Tabs } from '../../components/Tabs'
import { AtivoTable } from '../../features/carteira/components/AtivoTable'
import { CarteiraGraficos } from '../../features/carteira/components/CarteiraGraficos'
import { CarteiraSummary } from '../../features/carteira/components/CarteiraSummary'
import { ModalAdicionarAtivo } from '../../features/carteira/components/FormularioAtivo'
import { GraficoDesempenhoCarteira } from '../../features/carteira/components/GraficoDesempenho'
import { ModalImportadorB3 } from '../../features/carteira/components/ImportadorB3'
import { PainelOperacoesAtivo } from '../../features/carteira/components/PainelOperacoesAtivo'
import { EstadoSemCarteira, ModalCarteira, SeletorCarteira } from '../../features/carteira/components/SeletorCarteira'
import { PainelProventos } from '../../features/proventos/components/PainelProventos'

function obterIniciais(email) {
  if (!email) return '?'
  const base = email.split('@')[0] ?? ''
  return base.slice(0, 2).toUpperCase()
}

function Marca() {
  return (
    <div className="flex items-center gap-3">
      <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-violet-500/15 text-violet-300 ring-1 ring-inset ring-violet-500/30">
        <IconeCarteira />
      </div>
      <div className="leading-tight">
        <h1 className="text-base font-semibold tracking-tight text-white">CarteiraPro</h1>
        <p className="text-xs text-slate-400">Acompanhe seus investimentos</p>
      </div>
    </div>
  )
}

function PerfilUsuario({ usuario, logout }) {
  if (!usuario) {
    return (
      <Button variante="secondary" onClick={logout} className="inline-flex items-center gap-2">
        <IconeSair />
        Sair
      </Button>
    )
  }

  return (
    <div className="flex items-center gap-2 rounded-xl border border-slate-800 bg-slate-900/60 p-1.5 pl-2">
      <div
        className="flex h-8 w-8 items-center justify-center rounded-lg bg-slate-800 text-xs font-semibold text-slate-200"
        aria-hidden="true"
      >
        {obterIniciais(usuario.email)}
      </div>
      <div className="hidden flex-col leading-tight md:flex">
        <span className="text-xs text-slate-500">Conectado como</span>
        <span className="max-w-[180px] truncate text-sm font-medium text-slate-200" title={usuario.email}>
          {usuario.email}
        </span>
      </div>
      <Button
        variante="secondary"
        onClick={logout}
        className="inline-flex items-center gap-1.5 px-2.5 py-1.5 text-xs"
        aria-label="Sair"
        title="Sair"
      >
        <IconeSair />
        <span className="hidden sm:inline">Sair</span>
      </Button>
    </div>
  )
}

export function Dashboard({
  abas,
  abaAtiva,
  onMudarAba,
  cartoesResumo,
  cotacaoAtualizadaEm,
  linhasAtivos,
  dadosGraficos,
  desempenho,
  carregando,
  erro,
  formularioAtivo,
  importadorB3,
  carteiras,
  operacoesAtivo,
  painelProventos,
  excluirAtivo,
  excluirTodosAtivos,
  adicionarAtivoModal,
  importadorB3Modal,
  usuario,
  logout,
}) {
  const semCarteira = !carteiras.carregando && carteiras.carteiras.length === 0
  const bloqueado = !carteiras.carteiraAtiva
  const mensagemBloqueio = semCarteira
    ? 'Crie uma carteira para começar a cadastrar ativos.'
    : 'Selecione ou crie uma carteira antes de adicionar ativos.'

  const estadoVazio = <EstadoSemCarteira onCriar={carteiras.abrirNovaCarteira} />

  const paineis = {
    carteira: (
      <div className="space-y-6">
        {semCarteira ? (
          estadoVazio
        ) : (
          <>
            <CarteiraSummary cartoes={cartoesResumo} cotacaoAtualizadaEm={cotacaoAtualizadaEm} />

            <GraficoDesempenhoCarteira {...desempenho} />

            <AtivoTable
              linhas={linhasAtivos}
              excluirAtivo={excluirAtivo}
              excluirTodosAtivos={excluirTodosAtivos}
              abrirPainelOperacoes={operacoesAtivo.abrirPainel}
              abrirAdicionarAtivo={adicionarAtivoModal.abrir}
              abrirImportadorB3={importadorB3Modal.abrir}
              bloqueado={bloqueado}
              mensagemBloqueio={mensagemBloqueio}
            />

            <PainelOperacoesAtivo {...operacoesAtivo} />
            <ModalAdicionarAtivo aberto={adicionarAtivoModal.aberto} onFechar={adicionarAtivoModal.fechar} formularioAtivo={formularioAtivo} />
            <ModalImportadorB3 aberto={importadorB3Modal.aberto} onFechar={importadorB3Modal.fechar} importadorB3={importadorB3} />
          </>
        )}
      </div>
    ),
    graficos: semCarteira ? estadoVazio : <CarteiraGraficos dadosGraficos={dadosGraficos} />,
    proventos: semCarteira ? estadoVazio : <PainelProventos {...painelProventos} />,
  }

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="sticky top-0 z-30 border-b border-slate-800/80 bg-slate-950/85 backdrop-blur supports-[backdrop-filter]:bg-slate-950/70">
        <div className="mx-auto flex max-w-6xl flex-col gap-3 px-6 py-4 lg:flex-row lg:items-center lg:justify-between">
          <Marca />

          <div className="flex flex-wrap items-center gap-3 lg:justify-end">
            <SeletorCarteira carteiras={carteiras} />
            <div className="hidden h-8 w-px bg-slate-800 lg:block" />
            <PerfilUsuario usuario={usuario} logout={logout} />
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-6 py-8">
        {erro && (
          <Alert className="mb-8">
            {erro}
            <span className="mt-1 block text-sm text-red-400/80">Verifique se a API está rodando em http://localhost:5122</span>
          </Alert>
        )}

        {carregando ? (
          <p className="text-slate-400">Carregando...</p>
        ) : (
          <Tabs abas={abas} abaAtiva={abaAtiva} onMudarAba={onMudarAba} paineis={paineis} />
        )}
      </main>

      <ModalCarteira
        aberto={carteiras.modal.aberto}
        modo={carteiras.modal.modo}
        onFechar={carteiras.modal.fechar}
        formularioCarteira={carteiras.formularioCarteira}
      />
    </div>
  )
}
