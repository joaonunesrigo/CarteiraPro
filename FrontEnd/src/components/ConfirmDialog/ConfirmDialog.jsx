import { Button } from "../Button";

export function ConfirmDialog({
  titulo,
  mensagem,
  textoConfirmar,
  textoCancelar,
  variante,
  onConfirmar,
  onCancelar,
}) {
  const varianteConfirmar = variante === "perigo" ? "perigo" : "primary";

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center p-4"
      role="presentation"
    >
      <button
        type="button"
        className="absolute inset-0 bg-slate-950/70 backdrop-blur-sm"
        aria-label="Fechar"
        onClick={onCancelar}
      />

      <div
        role="alertdialog"
        aria-modal="true"
        aria-labelledby="dialogo-titulo"
        aria-describedby="dialogo-mensagem"
        className="relative w-full max-w-md rounded-xl border border-slate-700 bg-slate-900 p-6 shadow-xl"
      >
        <h2 id="dialogo-titulo" className="text-lg font-semibold text-white">
          {titulo}
        </h2>
        <p
          id="dialogo-mensagem"
          className="mt-2 text-sm leading-relaxed text-slate-300"
        >
          {mensagem}
        </p>

        <div className="mt-6 flex justify-end gap-3">
          <Button variante="secondary" type="button" onClick={onCancelar}>
            {textoCancelar}
          </Button>
          <Button
            variante={varianteConfirmar}
            type="button"
            onClick={onConfirmar}
          >
            {textoConfirmar}
          </Button>
        </div>
      </div>
    </div>
  );
}
