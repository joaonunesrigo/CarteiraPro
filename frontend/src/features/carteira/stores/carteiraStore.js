import { create } from 'zustand'

function dataHoje() {
  return new Date().toISOString().slice(0, 10)
}

const formularioAtivoInicial = {
  ticker: '',
  precoMedio: '',
  quantidade: '',
  tipo: 0,
}

const errosAtivoIniciais = {
  ticker: null,
  precoMedio: null,
  quantidade: null,
  tipo: null,
}

const formularioOperacaoInicial = {
  tipo: 0,
  data: dataHoje(),
  quantidade: '',
  precoUnitario: '',
  taxas: '',
  observacao: '',
}

export const useCarteiraStore = create((set) => ({
  carteiraAtivaId: null,
  setCarteiraAtivaId: (carteiraAtivaId) => set({ carteiraAtivaId }),

  formularioAtivo: formularioAtivoInicial,
  errosAtivo: errosAtivoIniciais,
  enviandoAtivo: false,
  setEnviandoAtivo: (enviandoAtivo) => set({ enviandoAtivo }),
  setCampoFormularioAtivo: (campo, valor) =>
    set((state) => ({
      formularioAtivo: { ...state.formularioAtivo, [campo]: valor },
      errosAtivo: { ...state.errosAtivo, [campo]: null },
    })),
  setErrosAtivo: (erros) => set((state) => ({ errosAtivo: { ...state.errosAtivo, ...erros } })),
  resetFormularioAtivo: () =>
    set({
      formularioAtivo: formularioAtivoInicial,
      errosAtivo: errosAtivoIniciais,
    }),

  importacaoAtivos: {
    linhas: [],
    processandoArquivo: false,
    erroArquivo: null,
    nomeArquivo: null,
  },
  setImportacaoAtivos: (dados) =>
    set((state) => ({
      importacaoAtivos: { ...state.importacaoAtivos, ...dados },
    })),
  atualizarLinhaImportacaoAtivo: (id, campo, valor) =>
    set((state) => ({
      importacaoAtivos: {
        ...state.importacaoAtivos,
        linhas: state.importacaoAtivos.linhas.map((linha) => (linha.id === id ? { ...linha, [campo]: valor } : linha)),
      },
    })),
  alternarAtivoSelecionadoImportacao: (id) =>
    set((state) => ({
      importacaoAtivos: {
        ...state.importacaoAtivos,
        linhas: state.importacaoAtivos.linhas.map((linha) =>
          linha.id === id && !linha.jaCadastrado ? { ...linha, selecionado: !linha.selecionado } : linha,
        ),
      },
    })),
  limparImportacaoAtivos: () =>
    set({
      importacaoAtivos: {
        linhas: [],
        processandoArquivo: false,
        erroArquivo: null,
        nomeArquivo: null,
      },
    }),

  ativoOperacoesSelecionado: null,
  formularioOperacao: formularioOperacaoInicial,
  abrirOperacoesAtivo: (ativo) =>
    set({
      ativoOperacoesSelecionado: ativo,
      formularioOperacao: formularioOperacaoInicial,
    }),
  fecharOperacoesAtivo: () =>
    set({
      ativoOperacoesSelecionado: null,
      formularioOperacao: formularioOperacaoInicial,
    }),
  setCampoFormularioOperacao: (campo, valor) =>
    set((state) => ({
      formularioOperacao: { ...state.formularioOperacao, [campo]: valor },
    })),
  resetFormularioOperacao: () => set({ formularioOperacao: formularioOperacaoInicial }),
}))
