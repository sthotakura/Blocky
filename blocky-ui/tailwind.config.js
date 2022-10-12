module.exports = {
    content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
    safelist: [
        'w-20',
        'h-20',
        'w-10',
        'h-10',
        'w-4',
        'h-4',
    ],
    darkMode: 'media', // or 'media' or 'class'
    theme: {
        extend: {
            borderWidth: {
                '16': '16px'
            },
            zIndex: {
                '9998': '9998'
            }
        },
    },
    variants: {
        textColor: ({after}) => after(['disabled']),
        borderColor: ({after}) => after(['disabled', 'active']),
        cursor: ({after}) => after(['disabled']),
        backgroundColor: ({after}) => after(['odd', 'even'])
    },
    plugins: [],
}
